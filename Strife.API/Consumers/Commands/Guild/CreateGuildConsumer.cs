using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;
using Strife.API.Contracts.Commands.Guild;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Interfaces;

namespace Strife.API.Consumers.Commands.Guild
{
    public class CreateGuildConsumer : IConsumer<ICreateGuild>
    {
        private readonly IGuildService _guildService;

        public CreateGuildConsumer(IGuildService guildService)
        {
            _guildService = guildService;
        }

        public async Task Consume(ConsumeContext<ICreateGuild> context)
        {
            await _guildService.AddAsync(new Configuration.Guild.Guild
            {
                Id = context.Message.GuildId,
                Name = context.Message.Name
            });

            await context.Publish<IGuildCreated>(new
            {
                GuildId = context.Message.GuildId,
                InitiatedBy = context.Message.InitiatedBy
            });
        }
    }
}