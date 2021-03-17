using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Strife.Configuration.User;

namespace Strife.Configuration.Guild
{
    public class GuildRole : IdentityRole<Guid>
    {
        public Guild Guild { get; set; }
    }
}