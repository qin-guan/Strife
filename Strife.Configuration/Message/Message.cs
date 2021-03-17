using System;

namespace Strife.Configuration.Message
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateEdited { get; set; }

        public User.StrifeUser Sender { get; set; }

        public Channel.Channel Channel { get; set; }
    }
}