using Domain.DomainModels;
using Domain.DTO;

namespace Services.Interface;

public interface ITicketService
{
    List<Ticket> GetAllTickets();
    Ticket? GetDetailsForTicket(Guid? id);
    void CreateNewTicket(Ticket t);
    void UpdateExistingTicket(Ticket t);
    void DeleteTicket(Guid id);
    AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
    bool AddToShoppingCart(AddToShoppingCartDto item, string? userId);
}