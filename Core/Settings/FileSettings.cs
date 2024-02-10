using Microsoft.AspNetCore.Hosting;

namespace Core.Settings;
public class FileSettings
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FileSettings(IWebHostEnvironment webHostEnvironment )
    {
        _webHostEnvironment = webHostEnvironment;
    }
    public const string ImagesPath = "/Assets/Images/";


    public const string AllowedExtensions = ".jpg,.png.,jpeg";
    public const int MaxFileSizeInMB = 5;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
}
