using Application.DTOs;
using Application.Errors;
using Application.Extensions;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<UserDTO> 
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).Password();
            }
        }

        public class Handler : IRequestHandler<Query, UserDTO>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJWTGenerator _jWTGenerator;
            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJWTGenerator tokenGen)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jWTGenerator = tokenGen;
            }

            public async Task<UserDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                    if (result.Succeeded)
                    {
                        //generate token
                        return new UserDTO
                        {
                            DisplayName = user.DisplayName,
                            UserName = user.UserName,
                            Image = user.Photos.FirstOrDefault(ph=>ph.IsMain)?.Id,
                            Token = _jWTGenerator.CreateToken(user)
                        };
                    }
                }
                throw new RestException(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
