using Application.DTOs;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class UserController: BaseController
    {
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(Login.Query query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(Register.Comand comand)
        {
            return await Mediator.Send(comand);
        }

        [HttpGet]
        public async Task<ActionResult<UserDTO>> CurrentUser()
        {
            return await Mediator.Send(new CurrentUser.Query());
        }
    }
}
