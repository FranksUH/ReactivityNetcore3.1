using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest
        {
            public string Id {get;set;}
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
                RuleFor(x => x.Date).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
                RuleFor(x => x.Venue).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private DataContext _dataContext;
            private IUserAccesor _userAccesor;
            public Handler(DataContext dataContext, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var newAct = new Activity
                {
                    Id = (request.Id == null || request.Id == "") ? Guid.NewGuid().ToString() : request.Id,
                    Category = request.Category,
                    City = request.City,
                    Date = request.Date,
                    Description = request.Description,
                    Title = request.Title,
                    Venue = request.Venue
                };

                _dataContext.Activities.Add(newAct);

                var user = _dataContext.Users.SingleOrDefault(u => u.UserName == _userAccesor.GetUserName());

                var attendee = new UserActivity
                {
                    ActivityId= newAct.Id,
                    AppUserId= user.Id,
                    IsHost= true,
                    DateJoined= DateTime.Now
                };

                _dataContext.UserActivities.Add(attendee);

                if (await _dataContext.SaveChangesAsync() > 1)//dos cambios por guardar
                    return Unit.Value;

                throw new Exception("Error saving Activity");
            }
        }

    }
}
