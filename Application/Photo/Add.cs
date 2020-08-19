using Application.Interfaces;
using Infrastructure;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Photo
{
    public class Add
    {
        public class Command : IRequest<Domain.Photo>
        { 
            public IFormFile File { get; set; }
        }
        public class Handler : IRequestHandler<Command, Domain.Photo>
        {
            private IPhotoAccesor _photoAccesor;
            private IUserAccesor _userAccesor;
            private DataContext _dataContext;
            public Handler(IPhotoAccesor photoAccesor, IUserAccesor userAccesor, DataContext dataContext)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
                _photoAccesor = photoAccesor;
            }
            public async Task<Domain.Photo> Handle(Command request, CancellationToken cancellationToken)
            {
                var photoId = await _photoAccesor.UploadPhoto(request.File);
                if (photoId == "Empty" || photoId == "Error")
                    throw new Exception("Error submitting file");

                var user = _dataContext.Users.FirstOrDefault(usr => usr.UserName == _userAccesor.GetUserName());
                var photo = new Domain.Photo()
                {
                    Id = photoId,
                    IsMain = (user.Photos.Count > 0)?false:true,
                    AppUserId = user.Id
                };

                _dataContext.Photos.Add(photo);

                if (_dataContext.SaveChanges() > 0)
                    return photo;
                throw new Exception("Error saving changes");
            }
        }
    }
}
