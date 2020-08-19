using Application.Photo;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class PhotoController: BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Domain.Photo>> Add([FromForm]Add.Command command)
        {
            return await Mediator.Send(command);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            return await Mediator.Send(new Delete.Command() { PhotoId=id});
        }
        [HttpPost("{id}/setmain")]
        public async Task<ActionResult<Unit>> SetMain(string id)
        {
            return await Mediator.Send(new SetMain.Command() { PhotoId=id});
        }
    }
}
