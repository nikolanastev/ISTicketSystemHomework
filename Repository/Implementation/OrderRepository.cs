using Domain.DomainModels;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class OrderRepository : IOrderRepository
{

    private readonly ApplicationDbContext _context;
    private readonly DbSet<Order> _orders;
    
    
    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
        _orders = context.Set<Order>();
    }


    public List<Order> GetAll()
    {
        return _orders
            .Include(z => z.TicketsInOrder)
            .Include("TicketsInOrder.Ticket")
            .Include(z => z.User)
            .ToListAsync().Result;
    }

    public Order? GetOrderById(Guid id)
    {
        return _orders
            .Include(z => z.TicketsInOrder)
            .Include("TicketsInOrder.Ticket")
            .Include(z => z.User)
            .SingleOrDefaultAsync(z => z.Id == id).Result;
    }
}