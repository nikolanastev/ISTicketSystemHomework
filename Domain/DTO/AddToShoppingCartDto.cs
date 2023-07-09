using Domain.DomainModels;

namespace Domain.DTO;

public class AddToShoppingCartDto
{
    public Ticket Ticket { get; set; }
    public Guid TicketId { get; set; }
    public int Quantity { get; set; }
}