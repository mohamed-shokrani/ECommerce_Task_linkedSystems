using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
  

}
