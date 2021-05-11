using System;
using Strife.Core.Guilds;
using Strife.Core.Users;

namespace Strife.Core.Joins
{
    public class GuildStrifeUser
    {
        public int Sequence { get; set; }

        public Guid GuildId { get; set; }
        public Guild Guild { get; set; }

        public Guid UserId { get; set; }
        public StrifeUser User { get; set; }
    }
}