using System;
using System.Collections.Generic;

using Strife.Configuration.User;
using Strife.Configuration.Joins;

namespace Strife.Configuration.Guild
{
    public class Guild
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<StrifeUser> Users { get; set; }
        public List<GuildStrifeUser> GuildStrifeUsers { get; set; }
        public List<Channel.Channel> Channels { get; set; }
    }
}