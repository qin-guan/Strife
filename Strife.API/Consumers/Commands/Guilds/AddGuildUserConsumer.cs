using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Contracts.Commands.Guilds;
using Strife.API.Contracts.Events.Guilds;
using Strife.Core.Database;
using Strife.Core.Joins;

namespace Strife.API.Consumers.Commands.Guilds
{
    public class AddGuildUserConsumer : IConsumer<IAddGuildUser>
    {
        private readonly StrifeDbContext _dbContext;

        public AddGuildUserConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task Consume(ConsumeContext<IAddGuildUser> context)
        {
            var guilds = _dbContext.GuildStrifeUsers.Where(gsu => gsu.UserId == context.Message.InitiatedBy);
            var sequence = !await guilds.AnyAsync() ? 0 : await guilds.MaxAsync(g => g.Sequence);

            var join = new GuildStrifeUser 
            {
                UserId = context.Message.InitiatedBy,
                GuildId = context.Message.GuildId,
                Sequence = sequence + 1
            };

            await _dbContext.GuildStrifeUsers.AddAsync(join);
            await _dbContext.SaveChangesAsync();

            await context.Publish<IGuildUserAdded>(new
            {
                context.Message.GuildId, context.Message.InitiatedBy
            });
        }
    }
}