using System;

namespace Strife.API.Consumers.Commands.Guilds
{
    public class GuildAddresses
    {
        public static readonly Uri AddGuildUserConsumer = new("queue:AddGuildUser");
        public static readonly Uri CreateGuildConsumer = new("queue:CreateGuild");
    }
}