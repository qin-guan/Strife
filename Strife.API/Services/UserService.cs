using System.Linq;
using System.Threading.Tasks;

using Strife.API.Interfaces;

using Strife.Configuration.User;
using Strife.Configuration.Guild;
using Strife.Configuration.Database;
using Strife.Configuration.Joins;

namespace Strife.API.Services
{
    public class UserService: IUserService
    {
        private readonly StrifeDbContext _context;
        public UserService(StrifeDbContext context)
        {
            _context = context;   
        }

        public int GetCurrentUserSequence(StrifeUser user)
        {
            var guilds = _context.GuildStrifeUser.Where(gsu => gsu.UserId == user.Id).ToList();
            if (guilds.Count() == 0) return 0;
            else return guilds.Max(g => g.Sequence);
        }
        public async Task JoinGuildAsync(StrifeUser user, Guild guild)
        {
            var join = new GuildStrifeUser 
            {
                User = user,
                Guild = guild,
                Sequence = this.GetCurrentUserSequence(user) + 1
            };

            await _context.GuildStrifeUser.AddAsync(join);
            await _context.SaveChangesAsync();
        }
    }
}