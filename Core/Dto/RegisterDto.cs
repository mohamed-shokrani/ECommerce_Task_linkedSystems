using System.ComponentModel.DataAnnotations;

namespace Core.Dto;
public class RegisterDto
{
    public string Address { get; set; }

    [Required,StringLength(100)]
    public string FirstName { get; set; }
    [Required, StringLength(100)]

    public string LastName { get; set; }
    [Required, StringLength(150)]

    public string Email { get; set; }
    [Required, StringLength(256)]

    public string Password { get; set; }

    [Required, StringLength(50)]

    public string UserName { get; set; }

}
