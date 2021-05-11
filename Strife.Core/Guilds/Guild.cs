using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Strife.Core.Channels;
using Strife.Core.Joins;
using Strife.Core.Resources;
using Strife.Core.Users;

namespace Strife.Core.Guilds
{
    public class Guild: IResource 
    {
        public ResourceType ResourceType => ResourceType.Guild;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<StrifeUser> Users { get; set; }
        public List<GuildStrifeUser> GuildStrifeUsers { get; set; }
        public List<Channel> Channels { get; set; }
    }
}