using System;
using Microsoft.AspNetCore.Identity;

namespace Strife.Configuration.User
{
    public class StrifeUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string DisplayName { get; set; }
    }
}
