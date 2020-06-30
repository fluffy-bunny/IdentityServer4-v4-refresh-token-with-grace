using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ReplaceEndpointRouter<T>(
            this IServiceCollection services) where T : class, IEndpointRouter
        {

            services.RemoveAll<IEndpointRouter>();
            services.AddTransient<IEndpointRouter, T>();
            return services;
        }

        public static IServiceCollection AddTenantServices(this IServiceCollection services)
        {
            services.AddScoped<ITenantData, TenantData>();
            services.ReplaceEndpointRouter<TenantAwareEndpointRouter>();
            return services;
        }

    }
}
