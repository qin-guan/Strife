using System;
using System.Collections.Generic;
using Automatonymous;
using MassTransit;
using Strife.API.Contracts.Commands.Guild;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Policies;
using Strife.API.Sagas.States;

namespace Strife.API.Sagas.StateMachines
{
    public class CreateGuildStateMachine : MassTransitStateMachine<CreateGuildState>
    {
        public State Started { get; private set; }
        public State AddedFirstRole{ get; private set; }
        public State AddedSecondRole { get; private set; }
        public State AddedUser { get; private set; }
        
        public Event<IGuildCreated> GuildCreatedEvent { get; private set; }
        public Event<IGuildRoleCreated> GuildRoleCreatedEvent { get; private set; }
        public Event<IGuildUserAdded> GuildUserAddedEvent { get; private set; }

        public CreateGuildStateMachine()
        {
            InstanceState(x => x.State, Started, AddedFirstRole, AddedSecondRole, AddedUser);

            Event(() => GuildCreatedEvent, e => e.CorrelateById(c => c.Message.GuildId));
            Event(() => GuildRoleCreatedEvent, e => e.CorrelateById(c => c.Message.GuildId));
            Event(() => GuildUserAddedEvent, e => e.CorrelateById(c => c.Message.GuildId));

            Initially(
                When(GuildCreatedEvent)
                    .TransitionTo(Started)
                    .Then(x => x.Instance.InitiatedBy = x.Data.InitiatedBy)
                    .SendAsync(new Uri("queue:CreateGuildRole"), context => context.Init<ICreateGuildRole>(new
                    {
                        GuildId = context.Instance.CorrelationId,
                        Name = "everyone",
                        AccessLevel = 1,
                        InternalRole = false,
                        Policies = new List<string>
                        {
                            GuildPolicies.CreateChannels,
                            GuildPolicies.ReadChannels,
                            GuildPolicies.UpdateChannels,
                            GuildPolicies.DeleteChannels,
                        },
                        InitiatedBy = context.Instance.InitiatedBy
                    }))
                    .SendAsync(new Uri("queue:CreateGuildRole"), context => context.Init<ICreateGuildRole>(new
                    {
                        GuildId = context.Instance.CorrelationId,
                        Name = "__owner",
                        AccessLevel = 0,
                        InternalRole = true,
                        Policies = GuildPolicies.GetAllPolicies(),
                        InitiatedBy = context.Instance.InitiatedBy
                    }))
                    .SendAsync(new Uri("queue:AddGuildUser"), context => context.Init<IAddGuildUser>(new
                    {
                        GuildId = context.Instance.CorrelationId,
                        InitiatedBy = context.Instance.InitiatedBy
                    }))
            );
            
            During(Started,
                When(GuildRoleCreatedEvent)
                    .TransitionTo(AddedFirstRole));
            During(Started,
                Ignore(GuildUserAddedEvent));
            
            During(AddedFirstRole,
                When(GuildRoleCreatedEvent)
                    .TransitionTo(AddedSecondRole));
            During(AddedFirstRole,
                Ignore(GuildUserAddedEvent));
            
            During(AddedSecondRole,
                When(GuildUserAddedEvent)
                    .TransitionTo(AddedUser));
        }
    }
}