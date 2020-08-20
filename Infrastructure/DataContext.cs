using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure
{
    public class DataContext: IdentityDbContext<AppUser>
    {
        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Domain.Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> Followings { get; set; }
        public DataContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserActivity>(uact=> uact.HasKey(ua => new { ua.ActivityId, ua.AppUserId }));
            builder.Entity<UserActivity>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(fk => fk.AppUserId);

            builder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(fk => fk.ActivityId);

            builder.Entity<UserFollowing>(uf => {
                uf.HasKey(k => new { k.ObserverId, k.TargetId });

                uf.HasOne(o => o.Observer)
                    .WithMany(u => u.Followings)
                    .HasForeignKey(fk => fk.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);

                uf.HasOne(t => t.Target)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(fk => fk.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
        
    }
}
