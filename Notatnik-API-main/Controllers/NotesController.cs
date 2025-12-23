using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotatnikAPI.Data;
using NotatnikAPI.Models;

namespace NotatnikAPI.Controllers;

[ApiController]
[Route("notes")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetNotes()
    {
        var userId = GetUserId();
        var notes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderBy(n => n.Id)
            .Select(n => new { id = n.Id, content = n.Content })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetNote(int id)
    {
        var userId = GetUserId();
        var note = await _context.Notes
            .AsNoTracking()
            .Where(n => n.Id == id && n.UserId == userId)
            .Select(n => new { id = n.Id, content = n.Content })
            .FirstOrDefaultAsync();

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> CreateNote([FromBody] CreateNoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        var note = new Note
        {
            Content = request.Content,
            UserId = userId
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, new { id = note.Id, content = note.Content });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        if (note.UserId != userId)
        {
            return StatusCode(403, "Forbidden: You don't have access to this note");
        }

        note.Content = request.Content;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var userId = GetUserId();
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        if (note.UserId != userId)
        {
            return StatusCode(403, "Forbidden: You don't have access to this note");
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

