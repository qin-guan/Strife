using System;

namespace Strife.API.Consumers.Commands.Messages
{
    public static class MessageAddresses
    {
        public static Uri CreateMessageConsumer = new("queue:CreateMessage");
    }
}