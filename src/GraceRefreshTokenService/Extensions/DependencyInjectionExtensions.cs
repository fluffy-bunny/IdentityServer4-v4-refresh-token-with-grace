using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGraceRefreshTokenService(
            this IServiceCollection services)
        {
            services.TryAddTransient<IRefreshTokenService, GraceRefreshTokenService>();

            return services;
        }
       
    }
}
