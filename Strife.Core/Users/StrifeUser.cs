using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Strife.Core.Joins;

namespace Strife.Core.Users
{
    public class StrifeUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string DisplayName { get; set; }

        public ICollection<Guilds.Guild> Guilds { get; set; }
        public List<GuildStrifeUser> GuildStrifeUsers { get; set; }
    }
}
