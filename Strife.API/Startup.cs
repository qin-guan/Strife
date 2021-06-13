using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Strife.API.Extensions;
using Strife.API.Hubs;
using Strife.API.Permissions;
using Strife.API.Providers;

using Strife.Core.Database;
using Strife.Core.Guilds;
using Strife.Core.Hostname;
using Strife.Core.Messages;
using Strife.Core.RabbitMQ;
using Strife.Core.Swagger;
using Strife.Core.Users;

namespace Strife.API
{
    public class Startup
    {
        public StrifeDbOptions StrifeDbOptions { get; private set; }
        public RabbitMqOptions RabbitMqOptions { get; private set; }
        public HostnameOptions HostnameOptions { get; private set; }
        public MessageOptions MessageOptions { get; private set; }
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
            RabbitMqOptions = Configuration.GetSection(RabbitMqOptions.RabbitMq).Get<RabbitMqOptions>();
            MessageOptions = Configuration.GetSection(MessageOptions.Message).Get<MessageOptions>();

            services.Configure<HostnameOptions>(Configuration.GetSection(HostnameOptions.Hostnames));
            services.Configure<MessageOptions>(Configuration.GetSection(MessageOptions.Message));

            services.AddControllers()
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddSwagger(HostnameOptions, new ApiVersion(0, 1, "alpha"), new OpenApiInfo { Title = "Strife.API" });

            services.AddSignalR();

            // Add database services
            services.AddDbContext<StrifeDbContext>(options =>
                options.UseNpgsql(StrifeDbOptions.ConnectionString,
                    builder => builder.MigrationsAssembly("Strife.Core")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddMassTransitRabbitMq(RabbitMqOptions);

            services.AddIdentityCore<StrifeUser>()
                .AddRoles<GuildRole>()
                .AddRoleManager<RoleManager<GuildRole>>()
                .AddEntityFrameworkStores<StrifeDbContext>();

            // Add identity services
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = HostnameOptions.Auth;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/hub")))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization();

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddSingleton<IUserIdProvider, NameIdUserIdProvider>();

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, GuidAndClaimsListAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseCors(config =>
                    config
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(HostnameOptions.Web)
                );

                app.UseSwaggerCore(new ApiVersion(0, 1, "alpha"), "Strife.API");
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<EventsHub>("/hub");
            });
        }
    }
}
