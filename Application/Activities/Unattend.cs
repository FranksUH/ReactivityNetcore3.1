using Application.Errors;
using Application.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Unattend
    {
        public class Command : IRequest
        { 
            public Guid Id { get; set; }
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
                var activity = await _dataContext.Activities.FindAsync(request.Id.ToString());

                if (activity == null)
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { activity = "Not Found" });

                var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserName == _userAccesor.GetUserName());

                var attendance = await _dataContext.UserActivities
                    .FirstOrDefaultAsync(ua => ua.AppUserId == user.Id && ua.ActivityId == activity.Id);

                if (attendance == null)
                    return Unit.Value;

                if(attendance.IsHost)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { attendance = "Host can't be removed" });
                _dataContext.UserActivities.Remove(attendance);

                if (_dataContext.SaveChanges() > 0)
                    return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
