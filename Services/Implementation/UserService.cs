using Domain.Identity;
using Repository.Interface;
using Services.Interface;

namespace Services.Implementation;

public class UserService : IUserService
{
    
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public bool ChangeUserRole(string? userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return false;
        user.Role = user.Role == Role.ROLE_ADMIN ? Role.ROLE_USER : Role.ROLE_ADMIN;
        _userRepository.Update(user);
        return true;
    }

    public List<CinemaApplicationUser> FindAll()
    {
        return _userRepository.GetAll() as List<CinemaApplicationUser> ?? new List<CinemaApplicationUser>();
    }

    public bool IsAdmin(string? userId)
    {
        var user = _userRepository.GetById(userId);
        return user?.Role == Role.ROLE_ADMIN;
    }
}