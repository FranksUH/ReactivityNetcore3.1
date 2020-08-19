using Application.DTOs;
using Application.Errors;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
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
            private DataContext _dataContext;
            private IMapper _mapper;

            public Handler(DataContext dataContext, IMapper mapper)
            {
                this._dataContext = dataContext;
                this._mapper = mapper;
            }
            public async Task<ActivityDTO> Handle(Query request, CancellationToken cancellationToken)
            {             
                var activity = await _dataContext.Activities
                    .FindAsync(request.Id);  //lazy loading
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" });

                var result = _mapper.Map<Activity, ActivityDTO>(activity);
                return result;
            }
        }
    }
}
