using Domain.DomainModels;

namespace Repository.Interface;

public interface IRepository<T> where T: BaseEntity
{
    IEnumerable<T> GetAll();
    T? GetById(Guid? id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}