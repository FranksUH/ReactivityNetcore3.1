using Application.Activities;
using Application.DTOs;
using Domain;
using System.Linq;

namespace Application
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<Activity, ActivityDTO>()
                .ForMember(d => d.Attendees, m => m.MapFrom(s => s.UserActivities));

            CreateMap<UserActivity, AttendeeDTO>()
                .ForMember(d => d.DisplayName, m => m.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Image, m => m.MapFrom(s => (s.AppUser.Photos.FirstOrDefault(ph => ph.IsMain) == null) ? null :
                            s.AppUser.Photos.FirstOrDefault(ph => ph.IsMain).Id))
                .ForMember(d => d.Following, m=> m.MapFrom<FollowingResolve>());

            CreateMap<Comment, CommentDTO>()
                .ForMember(d => d.UserName, m => m.MapFrom(com => com.Author.UserName))
                .ForMember(d => d.DisplayName, m => m.MapFrom(com => com.Author.DisplayName))
                .ForMember(d => d.Image, m => m.MapFrom(com => com.Author.Photos.FirstOrDefault(ph => ph.IsMain).Id));
        }
    }
}
