using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Profile
{
    public class Details
    {        
        public class Query : IRequest<ProfileDTO>
        {
            public string UserName { get; set; }
        }
        public class Handler : IRequestHandler<Query, ProfileDTO>
        {
            private readonly DataContext _dataContext;
            private readonly IUserAccesor _userAccesor;
            public Handler(DataContext dataContext, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }
            public async Task<ProfileDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == request.UserName);
                var requesterName = _userAccesor.GetUserName();
                return user.GetProfileDTOFor(requesterName);
            }
        }
    }    
}
