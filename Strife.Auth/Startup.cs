using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.Core.Database;
using Strife.Core.Hostname;
using Strife.Core.Swagger;
using Strife.Core.Users;

namespace Strife.Auth
{
    public class Startup
    {
        public StrifeDbOptions StrifeDbOptions { get; private set; }
        public HostnameOptions HostnameOptions { get; private set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            StrifeDbOptions = Configuration.GetSection(StrifeDbOptions.StrifeDb).Get<StrifeDbOptions>();
            HostnameOptions = Configuration.GetSection(HostnameOptions.Hostnames).Get<HostnameOptions>();

            services.Configure<HostnameOptions>(Configuration.GetSection(HostnameOptions.Hostnames));

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddSwagger(HostnameOptions, new ApiVersion(0, 1, "alpha"), new OpenApiInfo { Title = "Strife.Auth" });

            // Add database services
            services.AddDbContext<StrifeDbContext>(options =>
                options.UseNpgsql(StrifeDbOptions.ConnectionString,
                    builder => builder.MigrationsAssembly("Strife.Core")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            // Add identity services 
            services.AddDefaultIdentity<StrifeUser>()
                .AddEntityFrameworkStores<StrifeDbContext>();
            services.AddIdentityServer()
                .AddApiAuthorization<StrifeUser, StrifeDbContext>(options =>
                    {
                        options.Clients.AddSPA("Strife.Web", spa =>
                        {
                            spa
                            .WithRedirectUri(
                                HostnameOptions.GetHostnameWithPath(Hostname.Web, "/authentication/login-callback"))
                            .WithLogoutRedirectUri(
                                HostnameOptions.GetHostnameWithPath(Hostname.Web, "/authentication/logout-callback"));
                        });
                    });
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

                app.UseSwaggerCore(new ApiVersion(0, 1, "alpha"), "Strife.Auth");
            }

            app.UseSerilogRequestLogging();

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