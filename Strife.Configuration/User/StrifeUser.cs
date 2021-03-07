using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Strife.Configuration.Guild;
using Strife.Configuration.Joins;

namespace Strife.Configuration.User
{
    public class StrifeUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string DisplayName { get; set; }
        public ICollection<Guild.Guild> Guilds { get; set; }
        public List<GuildStrifeUser> GuildStrifeUsers { get; set; }
        public ICollection<GuildRole> Roles { get; set; }
    }
}
