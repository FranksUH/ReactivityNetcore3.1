using Application.DTOs;
using Application.Followers;
using Application.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/profile")]
    public class Follow : BaseController
    {
        [HttpPost("{userName}/follow")]
        public async Task<ActionResult<Unit>> Add(string userName)
        {
            return await Mediator.Send(new Add.Command {TargetUserName=userName});
        }

        [HttpDelete("{userName}/follow")]
        public async Task<ActionResult<Unit>> Delete(string userName)
        {
            return await Mediator.Send(new Delete.Command { TargetUserName = userName });
        }

        [HttpGet("{userName}/{predicate}")]
        public async Task<ActionResult<List<ProfileDTO>>> GetFollows(string userName, string predicate)
        {
            return await Mediator.Send(new List.Query() { Prdicate = predicate, UserName = userName });
        }
    }
}
