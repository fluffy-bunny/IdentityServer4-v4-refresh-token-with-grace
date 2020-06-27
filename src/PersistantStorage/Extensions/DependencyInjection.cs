using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PersistantStorage.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMyDefaultRefreshTokenStore(this IServiceCollection services)
        {
            services.TryAddTransient<IRefreshTokenStore, MyDefaultRefreshTokenStore>();
       
            return services;
        }
    }
}
