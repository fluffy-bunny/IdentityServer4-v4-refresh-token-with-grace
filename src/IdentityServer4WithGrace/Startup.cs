// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Services.Extensions;
using IdentityServer4.Validation;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PersistantStorage.Extensions;

namespace IdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScopedServices();

            //////////////////////////////////////////////
            // refresh_token grace feature begin
            //////////////////////////////////////////////
            services.AddMyDefaultRefreshTokenStore();
            services.AddBackgroundServices();
            services.AddGraceRefreshTokenService();
            //////////////////////////////////////////////
            // refresh_token grace feature end
            //////////////////////////////////////////////
            services.AddTransient<IResourceValidator, MyDefaultResourceValidator>();
            services.AddClaimsService<MyDefaultClaimsService>();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddExtensionGrantValidator<ArbitraryResourceOwnerGrantValidator>();

            builder.AddDeveloperSigningCredential();


            //////////////////////////////////////////////
            // IdentityServer sometimes doesn't do a TryAddTransient
            // so we have to replace the services with a remove then add.
            //////////////////////////////////////////////
            // replace IdentityServer's IClientSecretValidator with mine.
            // note: This isn't needed for the refesh_token grace stuff
            //       This is to allow a refresh_token to be redeemed without a client_secret
            services.ReplaceClientSecretValidator<MyClientSecretValidator>();

 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}