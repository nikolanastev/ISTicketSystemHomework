using Domain.DomainModels;

namespace Domain.DTO;

public class TicketDto
{
    public List<Ticket> Tickets { get; set; }
    public DateTime Date { get; set; }
}