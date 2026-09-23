using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesManagement.API.DTOs.Notes;
using NotesManagement.API.Services;

namespace NotesManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    /*
    |-------------------------------------------------------------------
    | GET ALL NOTES
    |-------------------------------------------------------------------
    */
    [HttpGet]
    public async Task<IActionResult> GetNotes()
    {
        var userId = GetCurrentUserId();

        var notes = await _noteService.GetNotesAsync(userId);

        return Ok(notes);
    }

    /*
    |-------------------------------------------------------------------
    | GET NOTE BY ID
    |-------------------------------------------------------------------
    */
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetNote(int id)
    {
        var userId = GetCurrentUserId();

        var note =
            await _noteService.GetNoteByIdAsync(
                id,
                userId
            );

        if (note is null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    /*
    |-------------------------------------------------------------------
    | CREATE NOTE
    |-------------------------------------------------------------------
    */
    [HttpPost]
    public async Task<IActionResult> CreateNote(
        CreateNoteRequest request)
    {
        var userId = GetCurrentUserId();

        var note =
            await _noteService.CreateNoteAsync(
                userId,
                request
            );

        return CreatedAtAction(
            nameof(GetNote),
            new { id = note.Id },
            note
        );
    }

    /*
    |-------------------------------------------------------------------
    | UPDATE NOTE
    |-------------------------------------------------------------------
    */
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateNote(
        int id,
        UpdateNoteRequest request)
    {
        var userId = GetCurrentUserId();

        var updated =
            await _noteService.UpdateNoteAsync(
                id,
                userId,
                request
            );

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /*
    |-------------------------------------------------------------------
    | DELETE NOTE
    |-------------------------------------------------------------------
    */
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var userId = GetCurrentUserId();

        var deleted =
            await _noteService.DeleteNoteAsync(
                id,
                userId
            );

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /*
    |-------------------------------------------------------------------
    | GET THE USER ID FORM JWT
    |-------------------------------------------------------------------
    */
    private int GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            throw new UnauthorizedAccessException(
                "User ID was not found in the authentication token."
            );
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user ID in authentication token."
            );
        }

        return userId;
    }
}