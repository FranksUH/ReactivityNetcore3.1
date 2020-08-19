using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Photo
{
    public class PhotoAccesor : IPhotoAccesor
    {
        private string path;
        public PhotoAccesor(IOptions<PhotoSettings> options)
        {
            path = options.Value.ContainerFolder;
        }

        public bool DeletePhoto(string photoId)
        {
            try
            {
                File.Delete($"{path}/{photoId}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
            }
        }

        public async Task<string> UploadPhoto(IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                try
                {
                    string id = Guid.NewGuid().ToString();
                    using (var stream = formFile.OpenReadStream())
                    {
                        FileStream fs = new FileStream($"{path}/{id}.jpg" , FileMode.Create, FileAccess.ReadWrite);
                        await formFile.CopyToAsync(fs);
                        fs.Close();
                    }
                    return id;
                }
                catch (Exception error)
                {
                    Console.WriteLine("Error uploading: " + error.Message);
                    return "Error";
                }                
            }
            return "Empty";
        }
    }
}
