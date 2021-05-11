using System.Threading.Tasks;
using MassTransit;
using Strife.API.Contracts.Commands.Channels;
using Strife.API.Contracts.Events.Channels;
using Strife.Core.Channels;
using Strife.Core.Database;

namespace Strife.API.Consumers.Commands.Channels
{
    public class CreateChannelConsumer : IConsumer<ICreateChannel>
    {
        private readonly StrifeDbContext _dbContext;

        public CreateChannelConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ICreateChannel> context)
        {
            var channel = await _dbContext.Channels.AddAsync(new Channel
            {
                Id = context.Message.ChannelId,
                Name = context.Message.Name,
                GroupName = context.Message.GroupName,
                GuildId = context.Message.GuildId,
                IsVoice = context.Message.IsVoice
            });
            await _dbContext.SaveChangesAsync();

            await context.Publish<IChannelCreated>(new
            {
                ChannelId = channel.Entity.Id,
                context.Message.GuildId,
                context.Message.InitiatedBy
            });
        }
    }
}