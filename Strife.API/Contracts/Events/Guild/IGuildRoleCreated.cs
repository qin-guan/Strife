using System;

namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildRoleCreated
    {
        public Guid InitiatedBy { get; }
    }
}