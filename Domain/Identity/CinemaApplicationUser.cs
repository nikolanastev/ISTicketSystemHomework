using Domain.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public class CinemaApplicationUser : IdentityUser
{
    public Role Role { get; set; } = Role.ROLE_USER;
    public virtual ShoppingCart ShoppingCart { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}