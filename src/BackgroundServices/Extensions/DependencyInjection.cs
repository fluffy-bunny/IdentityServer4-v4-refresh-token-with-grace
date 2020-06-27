using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Services.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService<DeleteRefreshTokenQueueItems>>();
            services.AddSingleton(typeof(IBackgroundTaskQueue<>), typeof(BackgroundTaskQueue<>));

            return services;
        }
    }
}
