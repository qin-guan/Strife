using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Strife.API.Permissions
{
    public class GuidAndClaimsList
    {
        public Guid Id { get; set; }
        public List<IdentityRoleClaim<Guid>> Claims { get; set; }
        
        public GuidAndClaimsList(Guid guildId, List<IdentityRoleClaim<Guid>> claims)
        {
            Id = guildId;
            Claims = claims;
        }
    }
}