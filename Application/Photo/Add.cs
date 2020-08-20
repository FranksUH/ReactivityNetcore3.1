using Application.Interfaces;
using Common.Service.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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
            private readonly IPhotoAccessor _photoAccesor;
            private readonly IUserAccesor _userAccesor;
            private readonly DataContext _dataContext;
            public Handler(IPhotoAccessor photoAccesor, IUserAccesor userAccesor, DataContext dataContext)
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

                var user = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == _userAccesor.GetUserName());
                var photo = new Domain.Photo()
                {
                    Id = photoId,
                    IsMain = (user.Photos.Count > 0) ? false : true,
                    AppUserId = user.Id
                };

                await _dataContext.Photos.AddAsync(photo);

                if (await _dataContext.SaveChangesAsync() > 0)
                    return photo;
                throw new Exception("Error saving changes");
            }
        }
    }
}
