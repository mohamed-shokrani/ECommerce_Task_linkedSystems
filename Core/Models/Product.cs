using System.ComponentModel.DataAnnotations;

namespace Core.Models;
public class Product :Entity
{ 
    [Required ]
    [StringLength(100)]
    public string Name { get; set; } 
    [StringLength(600)]

    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    [StringLength(300)]
    public string PhotoUrl { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]

    public string CreatedBy { get; set; }
    [Required]

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
}
