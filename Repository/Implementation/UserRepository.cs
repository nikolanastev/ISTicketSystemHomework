using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _context;
    private readonly DbSet<CinemaApplicationUser> _entities;
    

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<CinemaApplicationUser>();
    }
    
    public IEnumerable<CinemaApplicationUser?> GetAll()
    {
        return _entities.AsEnumerable();
    }

    public CinemaApplicationUser? GetById(string? id)
    {
        return _entities
            .Include(z => z.ShoppingCart)
            .Include("ShoppingCart.TicketsInShoppingCart")
            .Include("ShoppingCart.TicketsInShoppingCart.Ticket")
            .SingleOrDefault(user => user.Id == id);
    }

    public void Add(CinemaApplicationUser? user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _entities.Add(user);
        _context.SaveChanges();
    }

    public void Update(CinemaApplicationUser? user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _entities.Update(user);
        _context.SaveChanges();
    }

    public void Delete(CinemaApplicationUser? user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _entities.Remove(user);
        _context.SaveChanges();

    }
}