using Application.DTOs;
using Application.Profile;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class ProfileController:BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(string username)
        {
            return await Mediator.Send(new Application.Profile.Details.Query(){ UserName = username });
        }

        [HttpGet("{username}/activities")]
        public async Task<ActionResult<List<UserActivityDTO>>> GetActivities(string username, string predicate)
        {
            return await Mediator.Send(new ListActivities.Query { Predicate=predicate, Username=username});
        }
    }
}
