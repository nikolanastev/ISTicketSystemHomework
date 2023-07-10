using Domain.DTO;

namespace Services.Interface;

public interface IShoppingCartService
{
    ShoppingCartDto GetShoppingCartInfo(string? userId);
    bool DeleteTicketFromShoppingCart(string? userId, Guid id);
    bool OrderNow(string? userId);
}