using System.Threading.Tasks;
using MassTransit;
using Strife.API.Contracts.Commands.Guild;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Interfaces;

namespace Strife.API.Consumers.Commands.Guild
{
    public class AddGuildUserConsumer : IConsumer<IAddGuildUser>
    {
        private readonly IGuildService _guildService;

        public AddGuildUserConsumer(IGuildService guildService)
        {
            _guildService = guildService;
        }
        
        public async Task Consume(ConsumeContext<IAddGuildUser> context)
        {
            await _guildService.AddUserAsync(context.Message.GuildId, context.Message.InitiatedBy);

            await context.Publish<IGuildUserAdded>(new
            {
                GuildId = context.Message.GuildId,
                InitiatedBy = context.Message.InitiatedBy
            });
        }
    }
}