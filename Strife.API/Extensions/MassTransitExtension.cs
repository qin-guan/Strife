using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Strife.API.Consumers.Commands.Guilds;
using Strife.API.Consumers.Events.Guilds;
using Strife.API.Sagas.StateMachines;
using Strife.API.Sagas.States;
using Strife.Core.RabbitMQ;

namespace Strife.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitRabbitMq(this IServiceCollection services, RabbitMqOptions rabbitMqOptions)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.AddConsumers(typeof(Startup).Assembly);
                
                busConfig.AddSagaStateMachine<CreateGuildStateMachine, CreateGuildState>()
                    .InMemoryRepository()
                    .Endpoint(e =>
                    {
                        e.Name = "CreateGuild";
                    });

                // If you would like to use another bus, configure it here
                busConfig.UsingRabbitMq((context, config) =>
                {
                    config.Host(rabbitMqOptions.Host, "/", h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });

                    config.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();

            return services;
        }
    }
}