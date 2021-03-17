using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Strife.Configuration.Guild;
using Strife.Configuration.Database;
using Strife.API.Interfaces;

namespace Strife.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly StrifeDbContext _context;
        public RoleService(StrifeDbContext context)
        {
            _context = context;
        }

    }
}