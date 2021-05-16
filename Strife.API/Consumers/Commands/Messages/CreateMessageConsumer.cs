using System;
using System.Threading.Tasks;
using MassTransit;
using Strife.API.Contracts.Commands.Messages;
using Strife.API.Contracts.Events.Messages;
using Strife.Core.Database;
using Strife.Core.Messages;

namespace Strife.API.Consumers.Commands.Messages
{
    public class CreateMessageConsumer: IConsumer<ICreateMessage>
    {
        private readonly StrifeDbContext _dbContext;
        public CreateMessageConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
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
        }
    }
}