using Application.Errors;
using Application.Interfaces;
using Infrastructure;
using Infrastructure.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Photo
{
    public class Delete
    {
        public class Command : IRequest<bool>
        {
            public string PhotoId { get; set; }
        }
        public class Handler : IRequestHandler<Command, bool>
        {
            private IPhotoAccesor _photoAccesor;
            private DataContext _dataContext;
            public Handler(IPhotoAccesor photoAccesor, DataContext dataContext)
            {
                _dataContext = dataContext;
                _photoAccesor = photoAccesor;
            }
            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var photo = await _dataContext.Photos.FindAsync(request.PhotoId);
                if (photo == null)
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { Errors = "Photo not found" });

                if (_photoAccesor.DeletePhoto(request.PhotoId))
                {                  
                    _dataContext.Photos.Remove(photo);
                    if (_dataContext.SaveChanges() > 0)
                        return true;
                    return false;
                }
                return false;
            }
        }

    }
}
