using Domain.DomainModels;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class Repository<T> : IRepository<T> where T: BaseEntity
{
    
    private readonly ApplicationDbContext _context;
    private DbSet<T> _entities;
    private string _errorMessage = string.Empty;


    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }


    public IEnumerable<T> GetAll()
    {
        return _entities.AsEnumerable();
    }

    public T? GetById(Guid? id)
    {
        return _entities.SingleOrDefault(s => s.Id == id);
    }

    public void Add(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _entities.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _entities.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _entities.Remove(entity);
        _context.SaveChanges();
    }
}