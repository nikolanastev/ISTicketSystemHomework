using Domain.DomainModels;
using Domain.DTO;
using Repository.Interface;
using Services.Interface;

namespace Services.Implementation;

public class TicketService : ITicketService
{
    
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
    private readonly IUserRepository _userRepository;


    public TicketService(IRepository<Ticket> ticketRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository, IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
        _userRepository = userRepository;
    }

    public List<Ticket> GetAllTickets()
    {
        return _ticketRepository.GetAll().ToList();
    }

    public Ticket? GetDetailsForTicket(Guid? id)
    {
        return _ticketRepository.GetById(id);
    }

    public void CreateNewTicket(Ticket t)
    {
        _ticketRepository.Add(t);
    }

    public void UpdateExistingTicket(Ticket t)
    {
        _ticketRepository.Update(t);
    }

    public void DeleteTicket(Guid id)
    {
        var ticket = _ticketRepository.GetById(id);
        _ticketRepository.Delete(ticket ?? throw new InvalidOperationException());
    }

    public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
    {

        var ticket = GetDetailsForTicket(id);
        var model = new AddToShoppingCartDto()
        {
            Ticket = ticket,
            TicketId = ticket.Id,
            Quantity = 1
        };
        return model;
    }

    public bool AddToShoppingCart(AddToShoppingCartDto item, string? userId)
    {
        var user = _userRepository.GetById(userId);
        var userShoppingCart = user.ShoppingCart;

        var ticket = this.GetDetailsForTicket(item.TicketId);
        if (ticket == null) return false;
        var itemToAdd = new TicketInShoppingCart
        {
            Id = Guid.NewGuid(),
            Ticket = ticket,
            TicketId = ticket.Id,
            ShoppingCart = userShoppingCart,
            ShoppingCartId = userShoppingCart.Id,
            Quantity = item.Quantity
        };
            
        _ticketInShoppingCartRepository.Add(itemToAdd);
        return true;

    }
}