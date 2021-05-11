using Automatonymous;
using MassTransit;
using Strife.API.Consumers.Commands.Guilds;
using Strife.API.Consumers.Commands.Permissions;
using Strife.API.Consumers.Commands.Roles;
using Strife.API.Contracts.Commands.Guilds;
using Strife.API.Contracts.Commands.Permissions;
using Strife.API.Contracts.Commands.Roles;
using Strife.API.Contracts.Events.Guilds;
using Strife.API.Contracts.Events.Roles;
using Strife.API.Permissions;
using Strife.API.Sagas.States;

namespace Strife.API.Sagas.StateMachines
{
    public class CreateGuildStateMachine : MassTransitStateMachine<CreateGuildState>
    {
        public State Started { get; private set; }
        public State CreatedFirstRole { get; private set; }
        public State CreatedSecondRole { get; private set; }
        public State AddedUser { get; private set; }
        public State AddedUserToFirstRole { get; private set; }
        public State Done { get; private set; }

        public Event<IGuildCreated> GuildCreatedEvent { get; private set; }
        public Event<IRoleCreated> GuildRoleCreatedEvent { get; private set; }
        public Event<IGuildUserAdded> GuildUserAddedEvent { get; private set; }
        public Event<IRoleUserAdded> GuildRoleUserAddedEvent { get; private set; }

        public CreateGuildStateMachine()
        {
            InstanceState(x => x.State, Started, CreatedFirstRole, CreatedSecondRole, AddedUser, AddedUserToFirstRole, Done);

            Event(() => GuildCreatedEvent, e => e.CorrelateById(c => c.Message.GuildId));
            Event(() => GuildRoleCreatedEvent, e => e.CorrelateById(c => c.Message.GuildId));
            Event(() => GuildUserAddedEvent, e => e.CorrelateById(c => c.Message.GuildId));
            Event(() => GuildRoleUserAddedEvent, e => e.CorrelateById(c => c.Message.GuildId));

            Initially(
                When(GuildCreatedEvent)
                    .TransitionTo(Started)
                    .Then(x => x.Instance.InitiatedBy = x.Data.InitiatedBy)
                    .SendAsync(RoleAddresses.CreateRoleConsumer, context => context.Init<ICreateRole>(new
                    {
                        GuildId = context.Instance.CorrelationId,
                        Name = "everyone",
                        AccessLevel = 1,
                        InternalRole = false,
                        context.Instance.InitiatedBy
                    }))
                    .SendAsync(RoleAddresses.CreateRoleConsumer, context => context.Init<ICreateRole>(new
                    {
                        GuildId = context.Instance.CorrelationId,
                        Name = "__owner",
                        AccessLevel = 0,
                        InternalRole = true,
                        context.Instance.InitiatedBy
                    }))
                    .SendAsync(GuildAddresses.AddGuildUserConsumer, context => context.Init<IAddGuildUser>(new
                    {
                        GuildId = context.Instance.CorrelationId, context.Instance.InitiatedBy
                    }))
            );

            During(Started,
                When(GuildRoleCreatedEvent)
                    .TransitionTo(CreatedFirstRole));
            During(Started,
                Ignore(GuildUserAddedEvent));

            During(CreatedFirstRole,
                When(GuildRoleCreatedEvent)
                    .TransitionTo(CreatedSecondRole)
                    .SendAsync(PermissionAddresses.CreatePermissionsConsumer, context =>
                        context.Init<ICreatePermissions>(new
                        {
                            GuildId = context.Instance.CorrelationId,
                            RoleName = "__owner",
                            PermissionStrings = Permission.AllowAll(context.Instance.CorrelationId),
                            context.Instance.InitiatedBy,
                        }))
                    .SendAsync(PermissionAddresses.CreatePermissionsConsumer, context =>
                        context.Init<ICreatePermissions>(new
                        {
                            GuildId = context.Instance.CorrelationId,
                            RoleName = "everyone",
                            PermissionStrings = Permission.AllowAll(context.Instance.CorrelationId),
                            context.Instance.InitiatedBy,
                        })));
            During(CreatedFirstRole, 
                Ignore(GuildUserAddedEvent));
            
            During(CreatedSecondRole,
                When(GuildUserAddedEvent)
                    .TransitionTo(AddedUser)
                    .SendAsync(RoleAddresses.AddRoleUserConsumer, context => context.Init<IAddRoleUser>(new
                    {
                        RoleName = "everyone",
                        GuildId = context.Instance.CorrelationId,
                        context.Instance.InitiatedBy
                    }))
                    .SendAsync(RoleAddresses.AddRoleUserConsumer, context => context.Init<IAddRoleUser>(new
                    {
                        RoleName = "__owner",
                        GuildId = context.Instance.CorrelationId,
                        context.Instance.InitiatedBy
                    })));
            
            During(AddedUser,
                When(GuildRoleUserAddedEvent)
                    .TransitionTo(AddedUserToFirstRole));
            
            During(AddedUserToFirstRole,
                When(GuildRoleUserAddedEvent)
                    .TransitionTo(Done));
        }
    }
}