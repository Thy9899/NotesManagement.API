namespace NotesManagement.API.DTOs.Notes;

public class CreateNoteRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }
}
