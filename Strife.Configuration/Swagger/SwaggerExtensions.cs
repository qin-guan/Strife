using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using Strife.Configuration.Hostname;
using Strife.Configuration.Identity;

namespace Strife.Configuration.Swagger
{
    public static class SwaggerExtensions
    {
        // `UseSwaggerCore` is used to avoid conflicts with `UseSwagger`
        public static IApplicationBuilder UseSwaggerCore(this IApplicationBuilder app, ApiVersion version, string projectName)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{version.ToString()}/swagger.json", $"{projectName} {version}");
                c.OAuthClientId("Strife.Web");
                c.OAuthAppName("Strife.Auth");
                c.OAuthUsePkce();
            });

            return app;
        }
        public static IServiceCollection AddSwagger(this IServiceCollection services, HostnameOptions hostnameOptions, ApiVersion version, OpenApiInfo openApiInfo)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version.ToString(), openApiInfo);

                // Add identity configs
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                hostnameOptions.GetUriHostnameWithPath(Hostname.Hostname.Auth, "/connect/authorize"),
                            TokenUrl = hostnameOptions.GetUriHostnameWithPath(Hostname.Hostname.Auth, "/connect/token"),
                            Scopes = OAuthOptions.Scopes
                        }
                    }
                });
                
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }
    }
}