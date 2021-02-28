using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Strife.API.Consumers.Guild;

namespace Strife.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitCore(this IServiceCollection services)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.AddConsumer<CreateGuildConsumer>();
                
                // If you would like to use another bus, configure it here
                busConfig.UsingInMemory((context, config) =>
                {
                    config.ReceiveEndpoint("guild-events", endpointConfig =>
                    {
                        endpointConfig.ConfigureConsumer<CreateGuildConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

            return services;
        }
    }
}