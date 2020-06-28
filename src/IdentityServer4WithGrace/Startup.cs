// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Services.Extensions;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

            //////////////////////////////////////////////
            // refresh_token grace feature begin
            //////////////////////////////////////////////
            services.AddMyDefaultRefreshTokenStore();
            services.AddBackgroundServices();
            services.AddGraceRefreshTokenService();
            //////////////////////////////////////////////
            // refresh_token grace feature end
            //////////////////////////////////////////////

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()

                .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "interactive.public";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });


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