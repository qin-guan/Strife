using System;
using System.Collections.Generic;
using Strife.Configuration.User;

namespace Strife.Configuration.Guild
{
    public class Guild
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public ICollection<StrifeUser> Users { get; set; } 
    }
}