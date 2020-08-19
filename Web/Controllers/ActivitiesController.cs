using Application.Activities;
using Application.DTOs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{    
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        [HttpGet("{limit}/{offset}")]
        public async Task<ActionResult<List.ActivitiesEnvelope>> List(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
        {
            return await Mediator.Send(new List.Query(limit, offset, isGoing, isHost, startDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDTO>> Details(string id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command activity)
        {
            return await Mediator.Send(activity);
        }

        [HttpPut("{Id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(string Id, Edit.Command control)
        {
            control.Id = Id;
            return await Mediator.Send(control);
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(string Id)
        {
            return await Mediator.Send(new Delete.Control { Id = Id});
        }

        [HttpPost("{Id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid Id)
        {
            return await Mediator.Send(new Attend.Command{ Id = Id });
        }

        [HttpDelete("{Id}/attend")]
        public async Task<ActionResult<Unit>> Unattend(Guid Id)
        {
            return await Mediator.Send(new Unattend.Command { Id = Id });
        }
    }
}