using Microsoft.AspNetCore.Hosting;

namespace Core.Settings;
public class FileSettings
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public  static string _imagesPath;
    public FileSettings(IWebHostEnvironment webHostEnvironment )
    {
        _webHostEnvironment = webHostEnvironment;
        _imagesPath = _webHostEnvironment.WebRootPath;
    }
    public const string ImagesPath = "/Assets/Images/";

    public static string test = _imagesPath;

    public const string AllowedExtensions = ".jpg,.png.,jpeg";
    public const int MaxFileSizeInMB = 1;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
}
