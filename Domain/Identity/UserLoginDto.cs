using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Identity;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Email address is not valid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DisplayName("Remember me")]
    public bool RememberMe { get; set; }
}