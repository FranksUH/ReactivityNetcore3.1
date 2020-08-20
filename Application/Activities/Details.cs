using Application.DTOs;
using Application.Errors;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ActivityDTO>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDTO>
        {
            private readonly DataContext _dataContext;
            private readonly IMapper _mapper;

            public Handler(DataContext dataContext, IMapper mapper)
            {
                _dataContext = dataContext;
                _mapper = mapper;
            }
            public async Task<ActivityDTO> Handle(Query request, CancellationToken cancellationToken)
            {             
                var activity = await _dataContext.Activities
                    .FindAsync(request.Id);
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" });

                var result = _mapper.Map<Activity, ActivityDTO>(activity);
                return result;
            }
        }
    }
}
