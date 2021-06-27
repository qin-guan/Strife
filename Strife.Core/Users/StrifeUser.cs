using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Strife.Core.Guilds;
using Strife.Core.Joins;

namespace Strife.Core.Users
{
    public class StrifeUser : IdentityUser<Guid>
    {
        [Required] [PersonalData] public string DisplayName { get; set; }

        public ICollection<Guild> Guilds { get; set; }
        public List<GuildStrifeUser> GuildStrifeUsers { get; set; }
        public List<GuildInvitation> GuildInvitations { get; set; }
    }
}