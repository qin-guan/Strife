using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Strife.Configuration.User;

namespace Strife.Configuration.Guild
{
    public class GuildRole : IdentityRole<Guid>
    {
        public Guid GuildId { get; set; }
        public Guild Guild { get; set; }
        
        public int AccessLevel { get; set; }
        public bool InternalRole { get; set; }
    }
}