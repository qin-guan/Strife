using System;

namespace Strife.API.Consumers.Commands.Channels
{
    public static class ChannelAddresses
    {
        public static Uri CreateChannelConsumer = new("queue:CreateChannel");
    }
}