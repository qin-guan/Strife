using System;
using System.Collections.Generic;
using Strife.Core.Users;

namespace Strife.Core.Guilds
{
    public class GuildInvitation
    {
        public Guid Id { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Redemptions { get; set; }
        
        public Guid GuildId { get; set; }
        public Guild Guild { get; set; }
        public List<StrifeUser> RedeemedUsers { get; set; }
    }
}