using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class List 
    {
        public class ActivitiesEnvelope
        { 
            public List<ActivityDTO> Activities { get; set; }
            public int ActivityCount { get; set; }
        }

        public class Query : IRequest<ActivitiesEnvelope>
        {
            public int? Limit { get; set; }
            public int? Offset { get; set; }
            public bool IsGoing { get; set; }
            public bool IsHost { get; set; }
            public DateTime? StartDate { get; set; }

            public Query(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
            {
                Limit = limit;
                Offset = offset;
                IsGoing = isGoing;
                IsHost = isHost;
                StartDate = startDate ?? DateTime.Now;
            }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private DataContext _dataContext;
            private IMapper _mapper;
            private IUserAccesor _userAccesor;

            public Handler(DataContext dataContext, IMapper mapper, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _mapper = mapper;
                _userAccesor = userAccesor;
            }

            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _dataContext.Activities
                    .Include(a => a.UserActivities)  //eager loading
                    .ThenInclude(ua => ua.AppUser)
                    .Where(act=> act.Date >= request.StartDate && 
                          (!request.IsHost || act.UserActivities.Any(ua=>ua.AppUser.UserName == _userAccesor.GetUserName() && ua.IsHost)) &&
                          (!request.IsGoing || act.UserActivities.Any(ua=>ua.AppUser.UserName == _userAccesor.GetUserName())))
                    .OrderBy(act=>act.Date)
                    .AsQueryable();

                var filtered = await queryable.Skip(request.Offset ?? 0)
                                            .Take(request.Limit ?? 4)                    
                                            .ToListAsync();

                return new ActivitiesEnvelope
                {
                    Activities = _mapper.Map<List<Activity>, List<ActivityDTO>>(filtered),
                    ActivityCount = queryable.Count()
                };  
            }
        }

    }
}
