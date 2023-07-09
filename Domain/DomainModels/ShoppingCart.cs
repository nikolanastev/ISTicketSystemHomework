using Domain.Identity;

namespace Domain.DomainModels;

public class ShoppingCart : BaseEntity
{
    public string UserId { get; set; }
    public CinemaApplicationUser User { get; set; }
    public virtual ICollection<TicketInShoppingCart> TicketsInShoppingCart { get; set; }
}