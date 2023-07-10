using Domain.DomainModels;
using Repository.Interface;
using Services.Interface;

namespace Services.Implementation;

public class OrderService : IOrderService
{

    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }


    public List<Order> GetAllOrders()
    {
        return _orderRepository.GetAll();
    }

    public Order? GetOrderDetails(Guid orderId)
    {
        return _orderRepository.GetOrderById(orderId);
    }
}