using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class ProductCreateModel
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; } 
    [StringLength(600)]

    public required string Description { get; set; } 
    [Required]
    [Range(1, (double)decimal.MaxValue)]
    public  decimal Price { get; set; } 
    public IFormFile Image { get; set; } = default!;
}
