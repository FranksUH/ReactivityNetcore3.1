using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace Application.Activities
{
    public class FollowingResolve : IValueResolver<UserActivity, AttendeeDTO, bool>
    {
        private readonly DataContext _dataContext;
        private readonly IUserAccesor _userAccesor;
        public FollowingResolve(IUserAccesor userAccesor, DataContext dataContext)
        {
            _userAccesor = userAccesor;
            _dataContext = dataContext;
        }

        public bool Resolve(UserActivity source, AttendeeDTO destination, bool destMember, ResolutionContext context)
        {
            var currentUser = _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == _userAccesor.GetUserName()).Result;
            if (currentUser.Followings.Any(f => f.TargetId == source.AppUserId))
                return true;
            return false;
        }
    }
}
