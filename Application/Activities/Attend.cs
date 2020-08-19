using Application.DTOs;
using Application.Errors;
using Application.Interfaces;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Attend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; } //activity ID
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private IUserAccesor _userAccesor;
            private DataContext _dataContext;
            public Handler(IUserAccesor userAccesor, DataContext dataContext)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _dataContext.Activities.FindAsync(request.Id.ToString());

                if (activity == null)
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { activity= "Not Found"});

                var user = await _dataContext.Users.SingleOrDefaultAsync(u=>u.UserName == _userAccesor.GetUserName());

                var attendance = await _dataContext.UserActivities
                    .FirstOrDefaultAsync(ua => ua.AppUserId == user.Id && ua.ActivityId == activity.Id);

                if (attendance != null)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { attendance = "User alredy attending" });

                _dataContext.UserActivities.Add(new UserActivity
                {
                    ActivityId= activity.Id,
                    AppUserId= user.Id,
                    IsHost=false,
                    DateJoined=DateTime.Now
                });

                if (_dataContext.SaveChanges() > 0)
                    return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
