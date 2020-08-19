using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            private DataContext _dataContext;
            private IMapper _mapper;
            private IUserAccesor _userAccesor;
            public Handler(DataContext dataContext, IMapper mapper, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _mapper = mapper;
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
