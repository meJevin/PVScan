using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVScan.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Infrastructure.Data
{
    public class PVScanIdentityDbContext 
        : IdentityDbContext<
            User, IdentityRole<Guid>, Guid, 
            IdentityUserClaim<Guid>, IdentityUserRole<Guid>, 
            IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, 
            IdentityUserToken<Guid>>
    {
        public PVScanIdentityDbContext(DbContextOptions<PVScanIdentityDbContext> options)
            : base(options)
        {

        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(ConfigureUsers);
            builder.Entity<UserInfo>(ConfigureUserInfos);
            builder.Entity<UserSession>(ConfigureUserSessions);
            builder.Entity<RefreshToken>(ConfigureRefreshTokens);
        }

        private void ConfigureUsers(EntityTypeBuilder<User> builder)
        {
            builder
                .HasOne(a => a.Info)
                .WithOne(a => a.User)
                .HasForeignKey<UserInfo>(a => a.UserId);

            builder
                .HasMany(a => a.Sessions)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        }

        private void ConfigureUserInfos(EntityTypeBuilder<UserInfo> builder)
        {
        }

        private void ConfigureUserSessions(EntityTypeBuilder<UserSession> builder)
        {
            builder
                .HasOne(a => a.RefreshToken)
                .WithOne(a => a.UserSession)
                .HasForeignKey<RefreshToken>(a => a.UserSessionId);
        }

        private void ConfigureRefreshTokens(EntityTypeBuilder<RefreshToken> builder)
        {
        }
    }
}
