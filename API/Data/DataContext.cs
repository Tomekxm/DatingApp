using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating (ModelBuilder builder) 
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.userRoles)
                .WithOne(u => u.user)
                .HasForeignKey(u => u.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.userRoles)
                .WithOne(u => u.role)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();


            builder.Entity<UserLike>()
                .HasKey(k => new {k.sourceUserId, k.likedUserId});

            builder.Entity<UserLike>()
                .HasOne(s => s.sourceUser)
                .WithMany(l => l.likedUsers)
                .HasForeignKey(s => s.sourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserLike>()
                .HasOne(s => s.likedUser)
                .WithMany(l => l.likedByUsers)
                .HasForeignKey(s => s.likedUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Message>()
                .HasOne(u => u.recipient)
                .WithMany(m => m.messagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.sender)
                .WithMany(m => m.messagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}