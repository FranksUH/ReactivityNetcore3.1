using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Common.Service.Interfaces
{
    public interface IPhotoAccessor
    {
        Task<string> UploadPhoto(IFormFile formFile);
        bool DeletePhoto(string photoId);
    }
}
