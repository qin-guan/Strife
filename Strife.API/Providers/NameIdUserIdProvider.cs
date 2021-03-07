using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Strife.API.Providers
{
    public class NameIdUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.Claims.Single<Claim>(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}