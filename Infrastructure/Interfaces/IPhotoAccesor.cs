using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IPhotoAccesor
    {
        Task<string> UploadPhoto(IFormFile formFile);
        bool DeletePhoto(string photoId);
    }
}
