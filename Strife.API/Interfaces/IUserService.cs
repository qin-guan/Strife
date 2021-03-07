using System.Threading.Tasks;

using Strife.Configuration.User;
using Strife.Configuration.Guild;

namespace Strife.API.Interfaces
{
    public interface IUserService
    {
        public Task JoinGuildAsync(StrifeUser user, Guild guild);
    }
}