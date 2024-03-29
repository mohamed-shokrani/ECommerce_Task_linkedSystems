﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class ProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [StringLength(600)]

    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(1,(double) decimal.MaxValue)]
    public decimal Price { get; set; } 
    [StringLength(300)]
    public string PhotoUrl { get; set; } = string.Empty;
    

}
