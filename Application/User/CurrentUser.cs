using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User
{
    public class CurrentUser
    {
        public class Query : IRequest<UserDTO>{}

        public class Handler : IRequestHandler<Query, UserDTO>
        {
            private readonly IUserAccesor _userAccesor;
            private readonly IJWTGenerator _jWTGenerator;
            private readonly UserManager<AppUser> _userManager;
            public Handler(IUserAccesor userAccesor, UserManager<AppUser> userManager, IJWTGenerator jWTGenerator)
            {
                _userAccesor = userAccesor;
                _jWTGenerator = jWTGenerator;
                _userManager = userManager;
            }

            public async Task<UserDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccesor.GetUserName());
                return new UserDTO()
                {
                    DisplayName=user.DisplayName,
                    Image= user.Photos.FirstOrDefault(ph => ph.IsMain)?.Id,
                    UserName=user.UserName,
                    Token=_jWTGenerator.CreateToken(user)
                };
            }
        }
    }
}
