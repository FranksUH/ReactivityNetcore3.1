using Application.Errors;
using Application.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Photo
{
    public class SetMain
    {
        public class Command : IRequest
        {
            public string PhotoId { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private DataContext _dataContext;
            private IUserAccesor _userAccesor;
            public Handler(DataContext dataContext, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var photo = await _dataContext.Photos.FindAsync(request.PhotoId);
                if (photo == null)
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { error = "Photo not found" });
                if (photo.IsMain)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { error = "Photo already main" });

                var user = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == _userAccesor.GetUserName());

                var previousMain = user.Photos.FirstOrDefault(ph => ph.IsMain);        
                if(previousMain != null)
                    previousMain.IsMain = false;
                photo.IsMain = true;

                if (await _dataContext.SaveChangesAsync() > 0)
                    return Unit.Value;
                throw new Exception("Problem saving changes");
            }
        }
    }
}
