using System;

namespace Strife.API.Contracts.Commands.Roles
{
    public interface ICreateRole
    {
        public Guid GuildId { get; }
        public string Name { get; }
        public bool InternalRole { get; }
        public int AccessLevel { get; }
        public Guid InitiatedBy { get; }
    }
}