using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Strife.Core.Guilds;
using Strife.Core.Messages;
using Strife.Core.Resources;

namespace Strife.Core.Channels
{
    public class Channel: IResource
    {
        public ResourceType ResourceType => ResourceType.Channel;
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsVoice { get; set; }

        public string GroupName { get; set; }

        public Guid GuildId { get; set; }
        public Guild Guild { get; set; }
        public List<Message> Messages { get; set; }
    }
}