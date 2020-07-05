using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TenantServices.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITenantResolver _tenantResolver;
        private readonly ILogger _logger;

        public TenantMiddleware(RequestDelegate next, ITenantResolver tenantResolver, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _tenantResolver = tenantResolver;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context, ITenantData tenantData)
        {
            try
            {
                // TODO: Probably should use regex here.
                string[] parts = context.Request.Path.Value.Split('/');
                if (parts.Count() > 1)
                {
                    string tenantId = parts[1];
                    tenantData.TenantId = tenantId;
                    if (!_tenantResolver.IsTenantValid(tenantId))
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        await context.Response.WriteAsync("");
                        return;
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append('/');
                    if (parts.Count() > 2)
                    {
                        for (int i = 2; i < parts.Count(); i++)
                        {

                            sb.Append(parts[i]);
                            if (i < parts.Count() - 1)
                            {
                                sb.Append('/');
                            }
                        }
                    }
                    string newPath = sb.ToString();
                    context.Request.Path = newPath;
                    context.Request.PathBase = $"/{tenantId}";
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception: {exception}", ex.Message);
                throw;
            }

            await _next(context);
        }
    }
}
