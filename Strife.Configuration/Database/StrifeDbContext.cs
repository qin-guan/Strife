﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Strife.Configuration.Guild;
using Strife.Configuration.User;

namespace Strife.Configuration.Database
{
    public class StrifeDbContext : StrifeApiAuthorizationDbContext<StrifeUser, GuildRole, Guid>
    {
        public StrifeDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }
}