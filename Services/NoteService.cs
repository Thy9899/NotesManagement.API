using NotesManagement.API.DTOs.Notes;
using NotesManagement.API.Models;
using NotesManagement.API.Repositories;

namespace NotesManagement.API.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    /*
    |-------------------------------------------------------------------
    | GET ALL NOTES
    |-------------------------------------------------------------------
    */
    public async Task<IEnumerable<NoteResponse>> GetNotesAsync(int userId)
    {
        var notes = await _noteRepository.GetByUserIdAsync(userId);

        return notes.Select(MapToResponse);
    }

    /*
    |-------------------------------------------------------------------
    | GET NOTE BY ID
    |-------------------------------------------------------------------
    */
    public async Task<NoteResponse?> GetNoteByIdAsync(
        int id,
        int userId)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId);

        if (note is null)
        {
            return null;
        }

        return MapToResponse(note);
    }

    /*
    |-------------------------------------------------------------------
    | CREATE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<NoteResponse> CreateNoteAsync(
        int userId,
        CreateNoteRequest request)
    {
        var title = request.Title.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (title.Length > 200)
        {
            throw new ArgumentException(
                "Title cannot exceed 200 characters.");
        }

        var note = new Note
        {
            UserId = userId,
            Title = title,
            Content = request.Content?.Trim()
        };

        var noteId = await _noteRepository.CreateAsync(note);

        var createdNote =
            await _noteRepository.GetByIdAsync(noteId, userId);

        if (createdNote is null)
        {
            throw new InvalidOperationException(
                "Note was created but could not be retrieved.");
        }

        return MapToResponse(createdNote);
    }

    /*
    |-------------------------------------------------------------------
    | UPDATE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<NoteResponse?> UpdateNoteAsync( 

        int id, int userId, UpdateNoteRequest request) 
    { 
        var title = request.Title?.Trim(); 
            
        if (string.IsNullOrWhiteSpace(title)) { 

            throw new ArgumentException("Title is required."); 
        } 
        if (title.Length > 200) { 

            throw new ArgumentException( "Title cannot exceed 200 characters."); 
        } 
                    
        var note = new Note { 
            Id = id,
            UserId = userId, 
            Title = title, 
            Content = request.Content?.Trim() 
        }; 
                        
        var updated = await _noteRepository.UpdateAsync(note); 
            if (!updated) { 
                    
                return null; 
            } 
                        
        var updatedNote = await _noteRepository.GetByIdAsync(id, userId); 
                        
        if (updatedNote is null) { 

            throw new InvalidOperationException( "Note was updated but could not be retrieved."); 
        } 
                            
        return MapToResponse(updatedNote); 
    }
    

    /*
    |-------------------------------------------------------------------
    | DELETE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<bool> DeleteNoteAsync(
        int id,
        int userId)
    {
        return await _noteRepository.DeleteAsync(id, userId);
    }

    /*
    |-------------------------------------------------------------------
    | NoteResponse DTO
    |-------------------------------------------------------------------
    */
    private static NoteResponse MapToResponse(Note note)
    {
        return new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,

            CreatedAt = DateTime.SpecifyKind(
                note.CreatedAt,
                DateTimeKind.Utc
            ),

            UpdatedAt = DateTime.SpecifyKind(
                note.UpdatedAt,
                DateTimeKind.Utc
            )
        };
    }
}