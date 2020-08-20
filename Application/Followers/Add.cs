using Application.Errors;
using Application.Interfaces;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class Add
    {
        public class Command : IRequest
        { 
            public string TargetUserName { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IUserAccesor _userAccesor;
            private readonly DataContext _dataContext;

            public Handler(DataContext dataContext, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == _userAccesor.GetUserName());
                var target = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == request.TargetUserName);
                if (target == null)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Errors= "Target user not found" });

                if (await _dataContext.Followings.FirstOrDefaultAsync(f => f.ObserverId == user.Id && f.TargetId == target.Id) != null)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Errors = "Already Following" });

                var follow = new UserFollowing()
                {
                    ObserverId = user.Id,
                    TargetId = target.Id
                };
                _dataContext.Followings.Add(follow);

                if (_dataContext.SaveChanges() > 0)
                    return Unit.Value;

                throw new Exception("Something where wrong saving");
            }
        }
    }
}
