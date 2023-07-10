using System.Security.Claims;
using Domain.DomainModels;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interface;

namespace Web.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public ActionResult Index()
    {
        var tickets = new TicketDto
        {
            Tickets = _ticketService.GetAllTickets(),
            Date = DateTime.Now
        };
        return View(tickets);
    }

    [HttpPost]
    public IActionResult Index(TicketDto ticketDto)
    {
        var tickets = _ticketService.GetAllTickets()
            .Where(z => z.StartDate <= ticketDto.Date && z.EndDate >= ticketDto.Date).ToList();
        var model = new TicketDto
        {
            Tickets = tickets,
            Date = ticketDto.Date
        };
        return View(model);
    }

    public ActionResult Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ticket = _ticketService.GetDetailsForTicket(id);
        return ticket == null ? NotFound() : View(ticket);
    }

    public ActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind("MovieName,MovieYear,MovieGenre,MovieDescription,MovieImage,TicketPrice,StartDate,EndDate")] Ticket ticket)
    {
        if (!ModelState.IsValid) return View(ticket);
        this._ticketService.CreateNewTicket(ticket);
        return RedirectToAction(nameof(Index));
    }

    public ActionResult Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ticket = _ticketService.GetDetailsForTicket(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return View(ticket);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Guid id, [Bind("Id,MovieName,MovieYear,MovieGenre,MovieDescription,MovieImage,TicketPrice,StartDate,EndDate")] Ticket ticket)
    {
        if (id != ticket.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                this._ticketService.UpdateExistingTicket(ticket);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(ticket);
    }
    
    public ActionResult Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ticket = this._ticketService.GetDetailsForTicket(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return View(ticket);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(Guid id)
    {
        this._ticketService.DeleteTicket(id);
        return RedirectToAction(nameof(Index));
    }
    public IActionResult AddTicketToCart(Guid? id)
    {
        var model = this._ticketService.GetShoppingCartInfo(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTicketToCart([Bind("TicketId", "Quantity")] AddToShoppingCartDto item)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = this._ticketService.AddToShoppingCart(item, userId);

        if (result)
        {
            return RedirectToAction("Index", "Tickets");
        }

        return View(item);
    }
    
    private bool TicketExists(Guid id)
    {
        return this._ticketService.GetDetailsForTicket(id) != null;
    }
}