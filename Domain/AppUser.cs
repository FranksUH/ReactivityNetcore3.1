using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            UserActivities = new HashSet<UserActivity>();
            Photos = new HashSet<Photo>();
            Followings = new HashSet<UserFollowing>();
            Followers = new HashSet<UserFollowing>();
        }
        public string DisplayName { get; set; }
        public string Bios { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<UserFollowing> Followers { get; set; }
        public virtual ICollection<UserFollowing> Followings { get; set; }
    }
}