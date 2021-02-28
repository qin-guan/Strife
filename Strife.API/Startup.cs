using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Strife.API.Extensions;
using Strife.Configuration.Database;
using Strife.Configuration.Hostname;
using Strife.Configuration.RabbitMQ;
using Strife.Configuration.Swagger;

namespace Strife.API
{
    public class Startup
    {
        public StrifeDbOptions StrifeDbOptions { get; private set; }
        public RabbitMqOptions RabbitMqOptions { get; private set; }
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
            RabbitMqOptions = Configuration.GetSection(RabbitMqOptions.RabbitMq).Get<RabbitMqOptions>();

            services.Configure<HostnameOptions>(Configuration.GetSection(HostnameOptions.Hostnames));

            services.AddControllers();
            services.AddSwagger(HostnameOptions, new ApiVersion(0, 1, "alpha"), new OpenApiInfo { Title = "Strife.API" });

            // Add database services
            services.AddDbContext<StrifeDbContext>(options =>
                options.UseNpgsql(StrifeDbOptions.ConnectionString,
                    builder => builder.MigrationsAssembly("Strife.Configuration")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddMassTransitCore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

                app.UseSwaggerCore(new ApiVersion(0, 1, "alpha"), "Strife.API");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
