using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Strife.API.Providers
{
    public class NameIdUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            Debug.Assert(connection.User != null, "connection.User != null");
            return connection.User.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}