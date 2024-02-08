using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Core.Models;
public class AppUser :IdentityUser
{
    public string? Address { get; set; }
    [Required, MaxLength(128)]
    public string FisrtName { get; set; } = string.Empty;
    [Required, MaxLength(128)]

    public string LastName { get; set; } = string.Empty;
    public List<RefreshToken>? RefreshTokens { get; set; }
}
