using System.Collections;
using Domain.Identity;

namespace Repository.Interface;

public interface IUserRepository
{
    IEnumerable<CinemaApplicationUser?> GetAll();
    CinemaApplicationUser? GetById(string? id);
    void Add(CinemaApplicationUser? user);
    void Update(CinemaApplicationUser? user);
    void Delete(CinemaApplicationUser? user);
}