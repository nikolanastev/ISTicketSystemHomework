using Domain.DomainModels;

namespace Services.Interface;

public interface IOrderService
{
    List<Order> GetAllOrders();
    Order? GetOrderDetails(Guid orderId);
}