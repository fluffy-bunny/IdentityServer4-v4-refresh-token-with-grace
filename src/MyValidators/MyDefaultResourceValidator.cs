﻿using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Default implementation of IResourceValidator.
    /// </summary>
    public class MyDefaultResourceValidator : DefaultResourceValidator
    {
        private readonly IHttpContextRequestForm _httpContextRequestForm;
        private readonly ILogger _logger;

        public MyDefaultResourceValidator(
            IResourceStore store,
            IScopeParser scopeParser,
            IHttpContextRequestForm httpContextRequestForm,
            ILogger<DefaultResourceValidator> logger) : base(store, scopeParser, logger)
        {
            _httpContextRequestForm = httpContextRequestForm;
            _logger = logger;
        }

        /// <summary>
        /// Validates that the requested scopes is contained in the store, and the client is allowed to request it.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resourcesFromStore"></param>
        /// <param name="requestedScope"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override async Task ValidateScopeAsync(Client client, Resources resourcesFromStore, 
            ParsedScopeValue requestedScope, ResourceValidationResult result)
        {
            var parameters = await _httpContextRequestForm.GetFormCollectionAsync();
            var grantType = parameters.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType != "arbitrary_resource_owner")
            {
                await base.ValidateScopeAsync(client, resourcesFromStore, requestedScope, result);
            }
            else
            {
                if (requestedScope.ParsedName == IdentityServerConstants.StandardScopes.OfflineAccess)
                {
                    if (await IsClientAllowedOfflineAccessAsync(client))
                    {
                        result.Resources.OfflineAccess = true;
                        result.ParsedScopes.Add(new ParsedScopeValue(IdentityServerConstants.StandardScopes.OfflineAccess));
                    }
                    else
                    {
                        result.InvalidScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                    }
                }
                else
                {
                    result.ParsedScopes.Add(requestedScope);
                }
            }
        }

        /// <summary>
        /// Determines if client is allowed access to the API scope.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiScope"></param>
        /// <returns></returns>
        protected override async Task<bool> IsClientAllowedApiScopeAsync(Client client, ApiScope apiScope)
        {
            var parameters = await _httpContextRequestForm.GetFormCollectionAsync();
            var grantType = parameters.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType == "arbitrary_resource_owner")
            {
                return true;
            }
            return await base.IsClientAllowedApiScopeAsync(client, apiScope);
        }
    }
}

