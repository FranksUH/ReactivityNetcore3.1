using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User
{
    public class Register
    {
        public class Comand : IRequest<UserDTO>
        { 
            public string Email { get; set; }
            public string Password { get; set; }
            public string DisplayName { get; set; }
            public string UserName { get; set; }
        }

        public class ComandValidator : AbstractValidator<Comand>
        {
            public ComandValidator()
            {

                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).Password();
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Comand, UserDTO>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJWTGenerator _jWTGenerator;
            private readonly DataContext _dataContext;
            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DataContext dataContext ,IJWTGenerator jWTGenerator)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jWTGenerator = jWTGenerator;
                _dataContext = dataContext;
            }

            public async Task<UserDTO> Handle(Comand request, CancellationToken cancellationToken)
            {
                // if (await _dataContext.Users.AnyAsync(usr => usr.Email == request.Email))
                //     throw new RestException(System.Net.HttpStatusCode.BadRequest, new{Error= "User exists" });

                Console.WriteLine("INFO: "+request.Email + " " + request.DisplayName + " " + request.UserName);

                var user = new AppUser()
                {
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    UserName = request.UserName
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, true);
                    return new UserDTO
                    {
                        DisplayName=request.DisplayName,
                        Image=null,
                        UserName=request.UserName,
                        Token=_jWTGenerator.CreateToken(user)
                    };
                }
                throw new Exception("Problem creating user");
            }
        }
    }
}
