using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class ProductCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [StringLength(600)]

    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(1, (double)decimal.MaxValue)]
    public decimal Price { get; set; }
    public IFormFile ImageFile { get; set; }
}
