using System.Threading.Tasks;
using MassTransit;
using Serilog;
using Strife.API.Contracts.Commands.Guilds;
using Strife.API.Contracts.Events.Guilds;
using Strife.Core.Database;

namespace Strife.API.Consumers.Commands.Guilds
{
    public class CreateGuildConsumer : IConsumer<ICreateGuild>
    {
        private readonly StrifeDbContext _dbContext;

        public CreateGuildConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ICreateGuild> context)
        {
            await _dbContext.Guilds.AddAsync(new Core.Guilds.Guild
            {
                Id = context.Message.GuildId,
                Name = context.Message.Name
            });
            await _dbContext.SaveChangesAsync();

            await context.Publish<IGuildCreated>(new
            {
                context.Message.GuildId, context.Message.InitiatedBy
            });
        }
    }
}