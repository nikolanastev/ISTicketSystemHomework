using Domain.Identity;

namespace Services.Interface;

public interface IUserService
{
    bool ChangeUserRole(string? userId);
    List<CinemaApplicationUser> FindAll();
    bool IsAdmin(string? userId);
}