using Microsoft.AspNetCore.Http;
namespace Core.Interfaces;
public interface IImageService
{
   Task<string> UploadImage(IFormFile imageFile);
    Task<string> GetImageUrl(string imageFileName);
    Task DeleteOlderImage(string imageUrl);
}
