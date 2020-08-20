using Application.DTOs;
using Domain;
using System.Linq;

namespace Application.Extensions
{
    public static class ProfileReader
    {
        public static ProfileDTO GetProfileDTOFor(this AppUser appUser, string askingUser)
        {
            return new ProfileDTO()
            {
                Bio = appUser.Bios,
                DisplayName = appUser.DisplayName,
                FollowersCount = appUser.Followers.Count,
                FollowingCount = appUser.Followings.Count,
                Image = appUser.Photos.FirstOrDefault(ph=>ph.IsMain)?.Id,
                Photos=appUser.Photos,
                UserName = appUser.UserName,
                IsFollowed = appUser.Followers.Any(fw => fw.Observer.UserName == askingUser)
            };
        }
    }
}
