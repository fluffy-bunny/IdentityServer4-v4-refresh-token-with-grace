using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IHttpContextRequestForm, HttpContextRequestForm>();
            services.AddScoped<IOptionalClaims, OptionalClaims>();
            return services;
        }
    }
}
