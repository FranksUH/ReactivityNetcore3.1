using Application.DTOs;
using Application.Errors;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Profile
{
    public class ListActivities
    {
        public class Query : IRequest<List<UserActivityDTO>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<UserActivityDTO>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<UserActivityDTO>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

                if (user == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

                var queryable = user.UserActivities
                    .OrderBy(a => a.Activity.Date)
                    .AsQueryable();

                queryable = request.Predicate switch
                {
                    "past" => queryable.Where(a => a.Activity.Date <= DateTime.Now),
                    "hosting" => queryable.Where(a => a.IsHost),
                    _ => queryable.Where(a => a.Activity.Date >= DateTime.Now),
                };

                var activities = await queryable.ToListAsync();
                var activitiesToReturn = new List<UserActivityDTO>();

                foreach (var activity in activities)
                {
                    var userActivity = new UserActivityDTO
                    {
                        Id = Guid.Parse(activity.ActivityId),
                        Title = activity.Activity.Title,
                        Category = activity.Activity.Category,
                        Date = activity.Activity.Date
                    };

                    activitiesToReturn.Add(userActivity);
                }

                return activitiesToReturn;
            }
        }
    }
}
