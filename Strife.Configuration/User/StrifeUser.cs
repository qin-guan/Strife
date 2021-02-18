using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Strife.Configuration.User
{
    public class StrifeUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string DisplayName { get; set; }
        [PersonalData]
        public ICollection<Guild.Guild> Guilds { get; set; }
        [PersonalData]
        public ICollection<Guild.GuildRole> GuildRoles { get; set; }
    }
}
