using Domain.DomainModels;

namespace Domain.DTO;

public class ShoppingCartDto
{
    public List<Ticket> Tickets { get; set; }
    public DateTime Date { get; set; }
}