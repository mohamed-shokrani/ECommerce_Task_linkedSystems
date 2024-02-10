using Core.Interfaces;
using Core.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace Infrastructure.GenericRepository;
public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _imagesPath;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;

        _imagesPath = $"{_webHostEnvironment.WebRootPath}{FileSettings.ImagesPath}";
    }
    public async Task<string> UploadImage(IFormFile imageFile)
    {
        var imageUrl = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        var path = Path.Combine(_imagesPath, imageUrl);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return imageUrl;
    }

    public async Task DeleteOlderImage(string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            var path = Path.Combine(_imagesPath, imageUrl);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    public async Task<string> GetImageUrl(string imageFileName)
    {
        return $"{_webHostEnvironment.WebRootPath}{imageFileName}";
    }

}
