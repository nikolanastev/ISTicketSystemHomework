using Domain.DomainModels;
using Domain.DTO;
using Guid = System.Guid;
using System;
using System.Globalization;
using System.Text;
using Repository.Interface;
using Services.Interface;

namespace Services.Implementation;

public class ShoppingCartService : IShoppingCartService
{
    
    private readonly IRepository<ShoppingCart> _shoppingCartRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
    private readonly IRepository<EmailMessage> _emailMessageRepository;
    private readonly IUserRepository _userRepository;

    public ShoppingCartService (IRepository<ShoppingCart> shoppingCartRepository,
                                IRepository<Order> orderRepository,
                                IRepository<TicketInOrder> ticketInOrderRepository,
                                IRepository<EmailMessage> emailMessageRepository,
                                IUserRepository userRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _orderRepository = orderRepository;
        _ticketInOrderRepository = ticketInOrderRepository;
        _emailMessageRepository = emailMessageRepository;
        _userRepository = userRepository;
    }

    public ShoppingCartDto GetShoppingCartInfo(string? userId)
    {
        var loggedInUser = _userRepository.GetById(userId);
        var userShoppingCart = loggedInUser.ShoppingCart;
        var tickets = userShoppingCart.TicketsInShoppingCart.ToList();
        var totalPrice = tickets.Select(z => new
        {
            TicketPrice = z.Ticket.TicketPrice,
            Quantity = z.Quantity
        })
            .Aggregate(0.0, (_, z)=> z.TicketPrice * z.Quantity);

        var scDto = new ShoppingCartDto
        {
            Tickets = tickets,
            TotalPrice = totalPrice
        };
        
        return scDto;
    }

    public bool DeleteTicketFromShoppingCart(string? userId, Guid id)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        var loggedInUser = _userRepository.GetById(userId);
        var userShoppingCart = loggedInUser?.ShoppingCart;
        var itemToDelete = userShoppingCart?.TicketsInShoppingCart
            .FirstOrDefault(z => z.TicketId.Equals(id));
        
        userShoppingCart?.TicketsInShoppingCart.Remove(itemToDelete ?? throw new InvalidOperationException());
        _shoppingCartRepository.Update(userShoppingCart ?? throw new InvalidOperationException());
        
        return true;

    }

    public bool OrderNow(string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        var loggedInUser = _userRepository.GetById(userId);
        var userShoppingCart = loggedInUser?.ShoppingCart;

        var mail = new EmailMessage
        {
            MailTo = loggedInUser.Email,
            Subject = "Successfully ordered tickets",
            Status = false
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            User = loggedInUser,
            UserId = userId
        };

        _orderRepository.Add(order);

        var ticketInOrders = new List<TicketInOrder>();
            
        var result = userShoppingCart.TicketsInShoppingCart.Select(z => new TicketInOrder
        {
            Id = Guid.NewGuid(),
            TicketId = z.Ticket.Id,
            Ticket = z.Ticket,
            OrderId = order.Id,
            UserOrder = order,
            Quantity = z.Quantity
        }).ToList();

        var sb = new StringBuilder();
            

        sb.AppendLine("Your order is finished. This order contains: ");

        var totalPrice = result.Aggregate(0.0, (i, item) =>
        {
            sb.AppendLine(
                $"{i.ToString(CultureInfo.CurrentCulture)}. {item.Ticket.MovieName} {item.Ticket.MovieYear}, genre: {item.Ticket.MovieGenre} with price: {item.Ticket.TicketPrice} and amount : {item.Quantity}");
            return item.Quantity * item.Ticket.TicketPrice;
        });
            
        sb.AppendLine($"Total price: {totalPrice.ToString(CultureInfo.CurrentCulture)}");

        mail.Content = sb.ToString();

        ticketInOrders.AddRange(result);

        foreach (var item in ticketInOrders)
        {
            _ticketInOrderRepository.Add(item);
        }

        loggedInUser.ShoppingCart.TicketsInShoppingCart.Clear();

        _userRepository.Update(loggedInUser);
        _emailMessageRepository.Add(mail);
        return true;
    }
}
