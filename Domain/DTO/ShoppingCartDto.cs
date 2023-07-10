using Domain.DomainModels;

namespace Domain.DTO;

public class ShoppingCartDto
{
   public List<TicketInShoppingCart> Tickets { get; set; }
   public double TotalPrice { get; set; }
}