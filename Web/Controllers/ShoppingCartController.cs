using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Stripe;

namespace Web.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return View(_shoppingCartService.GetShoppingCartInfo(userId));
    }

    public IActionResult DeleteFromShoppingCart(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = _shoppingCartService.DeleteTicketFromShoppingCart(userId, id);

        return RedirectToAction("Index");
    }

    public IActionResult PayOrder(string stripeEmail, string stripeToken)
    {
        var customerService = new CustomerService();
        var chargeService = new ChargeService();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = this._shoppingCartService.GetShoppingCartInfo(userId);

        var customer = customerService.Create(new CustomerCreateOptions
        {
            Email = stripeEmail,
            Source = stripeToken
        });

        var charge = chargeService.Create(new ChargeCreateOptions
        {
            Amount = (Convert.ToInt32(order.TotalPrice) * 100),
            Description = "Ticket Payment",
            Currency = "mkd",
            Customer = customer.Id
        });

        if (charge.Status == "succeeded")
        {
            var result = this.Order();

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        return RedirectToAction("Index", "ShoppingCart");
    }

    private Boolean Order()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = this._shoppingCartService.OrderNow(userId);

        return result;
    }
}