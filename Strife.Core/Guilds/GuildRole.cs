using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Strife.Core.Resources;

namespace Strife.Core.Guilds
{
    public class GuildRole : IdentityRole<Guid>
    {
        public int AccessLevel { get; set; }
        public bool InternalRole { get; set; }

        public Guid GuildId { get; set; }
        public Guild Guild { get; set; }
    }
}