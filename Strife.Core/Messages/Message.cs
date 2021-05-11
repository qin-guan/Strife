using System;
using System.ComponentModel.DataAnnotations.Schema;
using Strife.Core.Channels;
using Strife.Core.Resources;
using Strife.Core.Users;

namespace Strife.Core.Messages
{
    public class Message: IResource
    {
        public ResourceType ResourceType => ResourceType.Message;
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateEdited { get; set; }

        public Guid SenderId { get; set; }
        public StrifeUser Sender { get; set; }

        public Channel Channel { get; set; }
    }
}