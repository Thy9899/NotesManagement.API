using NotesManagement.API.Models;

namespace NotesManagement.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<int> CreateAsync(User user);
}