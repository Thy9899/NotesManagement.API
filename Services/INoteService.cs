using NotesManagement.API.DTOs.Notes;

namespace NotesManagement.API.Services;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetNotesAsync(int userId);

    Task<NoteResponse?> GetNoteByIdAsync(int id, int userId);

    Task<NoteResponse> CreateNoteAsync(
        int userId,
        CreateNoteRequest request);

    Task<NoteResponse?> UpdateNoteAsync(
        int id,
        int userId,
        UpdateNoteRequest request);

    Task<bool> DeleteNoteAsync(
        int id,
        int userId);
}