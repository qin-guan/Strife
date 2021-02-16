using System.Collections.Generic;

namespace Strife.Configuration.Identity
{
    public static class OAuthOptions
    {
        public static readonly Dictionary<string, string> Scopes = new()
        {
            {"profile", "Profile"},
            {"openid", "OpenID"}
        };
    }
}