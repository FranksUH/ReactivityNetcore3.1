using Application.Errors;
using Common.Service.Interfaces;
using Infrastructure;
using MediatR;
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
            private readonly IPhotoAccessor _photoAccesor;
            private readonly DataContext _dataContext;
            public Handler(IPhotoAccessor photoAccesor, DataContext dataContext)
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
                    await Task.Run(()=> _dataContext.Photos.Remove(photo));
                    if (await _dataContext.SaveChangesAsync() > 0)
                        return true;
                    return false;
                }
                return false;
            }
        }
    }
}
