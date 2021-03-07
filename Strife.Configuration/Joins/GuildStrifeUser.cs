using System;

using Strife.Configuration.User;

namespace Strife.Configuration.Joins
{
    public class GuildStrifeUser
    {
        public int Sequence { get; set; }

        public Guid GuildId { get; set; }
        public Guild.Guild Guild { get; set; }

        public Guid UserId { get; set; }
        public StrifeUser User { get; set; }
    }
}