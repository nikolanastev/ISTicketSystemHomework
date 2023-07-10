using System.Security.Claims;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Domain.DomainModels;
using Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace Web.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly ITicketService _ticketService;
    private readonly UserManager<CinemaApplicationUser> _userManager;


    public AdminController(IUserService userService, ITicketService ticketService,
        UserManager<CinemaApplicationUser> userManager)
    {
        _userService = userService;
        _ticketService = ticketService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !_userService.IsAdmin(userId) ? RedirectToAction("Index", "Home") : View();
    }

    public IActionResult ImportUsers(IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!_userService.IsAdmin(userId))
        {
            return RedirectToAction("Index", "Home");
        }

        var pathToUpload = $"{Directory.GetCurrentDirectory()}\\Files\\{file.FileName}";

        using var fileStream = System.IO.File.Create(pathToUpload);
        file.CopyTo(fileStream);
        fileStream.Flush();

        var users = GetAllUsersFromFile(file.FileName);

        var status = true;
        foreach (var item in users)
        {
            var userCheck = _userManager.FindByEmailAsync(item.Email ?? throw new InvalidOperationException()).Result;
            Role role;
            role = item.UserRole == 0 ? Role.ROLE_ADMIN : Role.ROLE_USER;

            if (userCheck == null)
            {
                var user = new CinemaApplicationUser
                {
                    UserName = item.Email,
                    NormalizedUserName = item.Email,
                    Email = item.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    ShoppingCart = new ShoppingCart(),
                    Role = role
                };
                var result = _userManager.CreateAsync(user, item.Password ?? throw new InvalidOperationException())
                    .Result;
                status = status && result.Succeeded;
            }
            else
            {
                continue;
            }
        }

        return RedirectToAction("Index", "Action");
    }

    private IEnumerable<UserRegistrationDto> GetAllUsersFromFile(string fileName)
    {
        var users = new List<UserRegistrationDto>();
        var filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        while (reader.Read())
        {
            Role userRole;
            userRole = reader.GetValue(3).Equals("ROLE_ADMIN") ? Role.ROLE_ADMIN : Role.ROLE_USER;
            users.Add(new UserRegistrationDto
            {
                Email = reader.GetValue(0).ToString(),
                Password = reader.GetValue(1).ToString(),
                ConfirmPassword = reader.GetValue(2).ToString(),
                UserRole = userRole
            });
        }

        return users;
    }


    [HttpPost]
    public FileContentResult ExportTicketsFromGenre([Bind("MovieGenre")] Ticket ticket)
    {
        var genre = ticket.MovieGenre;

        string fileName = "tickets" + genre + ".xlsx";
        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        using (var workbook = new XLWorkbook())
        {
            IXLWorksheet worksheet = workbook.Worksheets.Add(genre.ToString());

            worksheet.Cell(1, 1).Value = "Ticket Id";
            worksheet.Cell(1, 2).Value = "Movie Name";
            worksheet.Cell(1, 3).Value = "Movie Year";
            worksheet.Cell(1, 4).Value = "Ticket Price";
            worksheet.Cell(1, 5).Value = "Streaming From";
            worksheet.Cell(1, 6).Value = "Streaming To";


            var result = this._ticketService.GetAllTickets().Where(z => z.MovieGenre == genre).ToList();

            if (result.Count > 0)
            {
                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.MovieName.ToString();
                    worksheet.Cell(i + 1, 3).Value = item.MovieYear.ToString();
                    worksheet.Cell(i + 1, 4).Value = item.TicketPrice.ToString();
                    worksheet.Cell(i + 1, 5).Value = item.StartDate.ToString();
                    worksheet.Cell(i + 1, 6).Value = item.EndDate.ToString();
                }
            }
            else
            {
                worksheet.Cell(2, 1).Value = "No tickets for selected genre";
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, contentType, fileName);
            }
        }
    }

    public FileContentResult ExportAllTickets()
    {
        const string fileName = "AllTickets.xlsx";
        const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        using var workbook = new XLWorkbook();
        IXLWorksheet worksheet = workbook.Worksheets.Add("AllTickets");

        worksheet.Cell(1, 1).Value = "Ticket Id";
        worksheet.Cell(1, 2).Value = "Movie Name";
        worksheet.Cell(1, 3).Value = "Movie Year";
        worksheet.Cell(1, 4).Value = "Movie Genre";
        worksheet.Cell(1, 5).Value = "Ticket Price";
        worksheet.Cell(1, 6).Value = "Streaming From";
        worksheet.Cell(1, 7).Value = "Streaming To";


        var result = _ticketService.GetAllTickets()
            .Where(z => z.StartDate <= DateTime.Now && z.EndDate >= DateTime.Now).ToList();

        if (result.Count > 0)
        {
            for (int i = 1; i <= result.Count(); i++)
            {
                var item = result[i - 1];

                worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                worksheet.Cell(i + 1, 2).Value = item.MovieName.ToString();
                worksheet.Cell(i + 1, 3).Value = item.MovieYear.ToString();
                worksheet.Cell(i + 1, 4).Value = item.MovieGenre.ToString();
                worksheet.Cell(i + 1, 5).Value = item.TicketPrice.ToString();
                worksheet.Cell(i + 1, 6).Value = item.StartDate.ToString();
                worksheet.Cell(i + 1, 7).Value = item.EndDate.ToString();
            }
        }
        else
        {
            worksheet.Cell(2, 1).Value = "No active tickets";
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, contentType, fileName);
    }
}