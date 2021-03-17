using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using IdentityServer4.EntityFramework.Options;

using Strife.Configuration.Guild;
using Strife.Configuration.User;
using Strife.Configuration.Joins;

namespace Strife.Configuration.Database
{
    public class StrifeDbContext : StrifeApiAuthorizationDbContext<StrifeUser, GuildRole, Guid>
    {
        public DbSet<Guild.Guild> Guilds { get; set; }
        public DbSet<Channel.Channel> Channels { get; set; }
        public DbSet<Message.Message> Messages { get; set; }

        public DbSet<GuildStrifeUser> GuildStrifeUser { get; set; }

        public StrifeDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guild.Guild>()
                .HasMany(g => g.Users)
                .WithMany(u => u.Guilds)
                .UsingEntity<GuildStrifeUser>(
                    j => j
                        .HasOne(gsu => gsu.User)
                        .WithMany(u => u.GuildStrifeUsers)
                        .HasForeignKey(gsu => gsu.UserId),
                    j => j
                        .HasOne(gsu => gsu.Guild)
                        .WithMany(g => g.GuildStrifeUsers)
                        .HasForeignKey(gsu => gsu.GuildId),
                    j => j.HasKey(gsu => new { gsu.GuildId, gsu.UserId })
                );
        }
    }
}