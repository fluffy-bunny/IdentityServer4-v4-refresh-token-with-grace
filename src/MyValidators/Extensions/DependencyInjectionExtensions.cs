using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ReplaceClientSecretValidator<T>(
            this IServiceCollection services) where T : class, IClientSecretValidator
        {

            services.RemoveAll<IClientSecretValidator>();
            services.AddTransient<IClientSecretValidator, T>();
            return services;
        }
        public static IServiceCollection AddClaimsService<T>(this IServiceCollection services) 
            where T : class, IClaimsService
        {
            services.AddTransient<IClaimsService, T>();
            return services;
        }

    }
}
