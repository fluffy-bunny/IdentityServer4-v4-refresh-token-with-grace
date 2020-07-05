using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTenantServices(this IServiceCollection services)
        {
            services.AddScoped<ITenantData, TenantData>();
            services.AddSingleton<ITenantResolver, TenantResolver>();
            return services;
        }
    }
}
