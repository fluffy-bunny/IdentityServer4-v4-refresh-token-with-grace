using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TenantServices.Extensions;

namespace IdentityServer4.Hosting
{
    public class TenantAwareEndpointRouter : IEndpointRouter
    {
        private readonly ITenantData _tenantData;
        private readonly IEnumerable<Endpoint> _endpoints;
        private readonly IdentityServerOptions _options;
        private readonly ILogger _logger;

        public TenantAwareEndpointRouter(ITenantData tenantData,IEnumerable<Endpoint> endpoints, IdentityServerOptions options, ILogger<TenantAwareEndpointRouter> logger)
        {
            _tenantData = tenantData;
            _endpoints = endpoints;
            _options = options;
            _logger = logger;
        }

        public IEndpointHandler Find(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string[] parts = context.Request.Path.Value.Split('/');
            if(parts.Count() > 1)
            {
                string tenantId = parts[1];
                StringBuilder sb = new StringBuilder();
                sb.Append('/');
                if(parts.Count() > 2)
                {
                    for(int i = 2; i<parts.Count();i++)
                    {
                        
                        sb.Append(parts[i]);
                        if(i < parts.Count() - 1)
                        {
                            sb.Append('/');
                        }
                    }
                }
                string newPath = sb.ToString();
                foreach (var endpoint in _endpoints)
                {
                    var path = endpoint.Path;
                    if (newPath.Equals(path, StringComparison.OrdinalIgnoreCase))
                    {
                        var endpointName = endpoint.Name;
                        _logger.LogDebug("Request path {path} matched to endpoint type {endpoint}", newPath, endpointName);
                        _tenantData.TenantId = tenantId;
                        var basePath = context.GetIdentityServerBasePath();
                        context.SetIdentityServerBasePath($"{basePath}/{tenantId}");
                        return GetEndpointHandler(endpoint, context);
                    }
                }

            }

            _logger.LogTrace("No endpoint entry found for request path: {path}", context.Request.Path);

            return null;
        }

        private IEndpointHandler GetEndpointHandler(Endpoint endpoint, HttpContext context)
        {
            if (_options.Endpoints.IsEndpointEnabled(endpoint))
            {
                if (context.RequestServices.GetService(endpoint.Handler) is IEndpointHandler handler)
                {
                    _logger.LogDebug("Endpoint enabled: {endpoint}, successfully created handler: {endpointHandler}", endpoint.Name, endpoint.Handler.FullName);
                    return handler;
                }

                _logger.LogDebug("Endpoint enabled: {endpoint}, failed to create handler: {endpointHandler}", endpoint.Name, endpoint.Handler.FullName);
            }
            else
            {
                _logger.LogWarning("Endpoint disabled: {endpoint}", endpoint.Name);
            }

            return null;
        }
    }
}
