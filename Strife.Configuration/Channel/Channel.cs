using System;
using System.Collections.Generic;

namespace Strife.Configuration.Channel
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsVoice { get; set; }

        public string GroupName { get; set; }

        public Guild.Guild Guild { get; set; }

        public List<Message.Message> Messages { get; set; }
    }
}