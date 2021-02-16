using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Strife.Configuration.User;
using Strife.Configuration.Database;
using Strife.Configuration.Hostname;
using Strife.Configuration.Swagger;

namespace Strife.Auth
{
    public class Startup
    {
        public StrifeDbOptions StrifeDbOptions { get; private set; }
        public HostnameOptions HostnameOptions { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            StrifeDbOptions = Configuration.GetSection(StrifeDbOptions.StrifeDb).Get<StrifeDbOptions>();
            HostnameOptions = Configuration.GetSection(HostnameOptions.Hostnames).Get<HostnameOptions>();

            services.Configure<HostnameOptions>(Configuration.GetSection(HostnameOptions.Hostnames));

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddSwagger(HostnameOptions, "1", new OpenApiInfo {Title = "Strife.Auth"});

            // Add database services
            services.AddDbContext<StrifeDbContext>(options =>
                options.UseNpgsql(StrifeDbOptions.ConnectionString,
                    builder => builder.MigrationsAssembly("Strife.Configuration")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            // Add identity services 
            services.AddDefaultIdentity<StrifeUser>()
                .AddEntityFrameworkStores<StrifeDbContext>();
            services.AddIdentityServer()
                .AddApiAuthorization<StrifeUser, StrifeDbContext>(options =>
                    options.Clients.AddSPA("Strife.Web",
                        spa => spa
                            .WithRedirectUri(
                                HostnameOptions.GetHostnameWithPath(Hostname.Web, "/authentication/login-callback"))
                            .WithLogoutRedirectUri(
                                HostnameOptions.GetHostnameWithPath(Hostname.Web, "/authentication/logout-callback"))
                    ));
            services.AddAuthentication()
                .AddIdentityServerJwt();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

                app.UseSwaggerCore("1", "Strife.Auth");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}