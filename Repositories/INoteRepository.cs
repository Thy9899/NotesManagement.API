using NotesManagement.API.Models;

namespace NotesManagement.API.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetByUserIdAsync(int userId);

    Task<Note?> GetByIdAsync(int id, int userId);

    Task<int> CreateAsync(Note note);

    Task<bool> UpdateAsync(Note note);

    Task<bool> DeleteAsync(int id, int userId);
}
