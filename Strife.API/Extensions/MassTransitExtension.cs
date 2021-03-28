using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

using Strife.API.Consumers.Commands.Guild;
using Strife.API.Consumers.Events.Guild;
using Strife.API.Contracts.Commands.Guild;
using Strife.API.Sagas.StateMachines;
using Strife.API.Sagas.States;
using Strife.Configuration.RabbitMQ;

namespace Strife.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitRabbitMq(this IServiceCollection services, RabbitMqOptions rabbitMqOptions)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.AddConsumersFromNamespaceContaining<CreateGuildConsumer>();
                busConfig.AddConsumersFromNamespaceContaining<GuildCreatedConsumer>();
                
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