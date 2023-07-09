using Domain.Identity;

namespace Domain.DomainModels;

public class Order : BaseEntity
{
    public string UserId { get; set; }
    public CinemaApplicationUser User { get; set; }
    public IEnumerable<TicketInOrder> TicketsInOrder { get; set; }
}