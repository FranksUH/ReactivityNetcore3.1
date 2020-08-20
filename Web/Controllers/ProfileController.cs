using Application.DTOs;
using Application.Profile;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class ProfileController:BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(string username)
        {
            return await Mediator.Send(new Details.Query(){ UserName = username });
        }

        [HttpGet("{username}/activities")]
        public async Task<ActionResult<List<UserActivityDTO>>> GetActivities(string username, string predicate)
        {
            return await Mediator.Send(new ListActivities.Query { Predicate=predicate, Username=username});
        }
    }
}
