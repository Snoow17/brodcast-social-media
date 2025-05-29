using BrodcastSocialMedia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BrodcastSocialMedia.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Broadcast> Broadcasts { get; set; }
        public DbSet<BroadcastLike> BroadcastLikes { get; set; }

        public DbSet<UserListening> UserListenings { get; set; } // Add this

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserListening>()
                .HasKey(ul => new { ul.ListenerId, ul.TargetId });

            builder.Entity<UserListening>()
                .HasOne(ul => ul.Listener)
                .WithMany(u => u.ListeningTo)
                .HasForeignKey(ul => ul.ListenerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserListening>()
                .HasOne(ul => ul.Target)
                .WithMany(u => u.ListenedBy)
                .HasForeignKey(ul => ul.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
