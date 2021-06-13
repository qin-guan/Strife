using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Strife.API.Contracts.Commands.Messages;
using Strife.API.Contracts.Events.Channels;
using Strife.API.Contracts.Events.Messages;
using Strife.Core.Database;
using Strife.Core.Messages;

namespace Strife.API.Consumers.Commands.Messages
{
    public class CreateMessageConsumer : IConsumer<ICreateMessage>
    {
        private readonly StrifeDbContext _dbContext;
        private readonly MessageOptions _messageOptions;

        public CreateMessageConsumer(StrifeDbContext dbContext, IOptions<MessageOptions> messageOptions)
        {
            _dbContext = dbContext;
            _messageOptions = messageOptions.Value;
        }

        public async Task Consume(ConsumeContext<ICreateMessage> context)
        {
            var message = await _dbContext.Messages.AddAsync(new Message
            {
                ChannelId = context.Message.ChannelId,
                Content = context.Message.Content,
                DateSent = DateTime.Now,
                SenderId = context.Message.InitiatedBy,
            });
            await _dbContext.SaveChangesAsync();

            await context.Publish<IMessageCreated>(new
            {
                MessageId = message.Entity.Id,
                context.Message.GuildId,
                context.Message.ChannelId,
                context.Message.InitiatedBy
            });

            if (await _dbContext.Messages.Where(m => m.ChannelId == context.Message.ChannelId).LongCountAsync() %
                _messageOptions.PageSize == 1)
            {
                await context.Publish<IChannelMetaUpdated>(new
                {
                    context.Message.ChannelId,
                    context.Message.GuildId,
                    context.Message.InitiatedBy
                });
            }
        }
    }
}