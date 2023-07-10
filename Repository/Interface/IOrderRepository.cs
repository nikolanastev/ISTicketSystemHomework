using Domain.DomainModels;

namespace Repository.Interface;

public interface IOrderRepository
{
    List<Order> GetAll();
    Order? GetOrderById(Guid id);
}