using Core.Attributes;
using Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class ProductUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [StringLength(600)]

    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(1, (double)decimal.MaxValue)]
    public decimal Price { get; set; }
    [AllowedExtensions(FileSettings.AllowedExtensions), MaxFileSize(FileSettings.MaxFileSizeInBytes)]

    public IFormFile?  ImageFile { get; set; }  
}
