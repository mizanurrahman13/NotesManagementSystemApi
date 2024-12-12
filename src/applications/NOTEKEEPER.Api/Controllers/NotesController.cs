using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NOTEKEEPER.Api.Contexts;
using NOTEKEEPER.Api.Entities;
using NOTEKEEPER.Api.Enums;
using NOTEKEEPER.Api.Models;
using System.Security.Claims;

namespace NOTEKEEPER.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public NotesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var note = new Note
        {
            UserId = userId,
            Text = model.Text,
            Type = model.Type,
            Reminder = model.Reminder,
            DueDate = model.DueDate,
            IsComplete = model.IsComplete,
            Url = model.Url
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return Ok(note);
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var notes = await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
        return Ok(notes);
    }

    [HttpGet("dashboard/today")]
    [Authorize]
    public async Task<IActionResult> GetTodayNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var today = DateTime.Today;
        var notes = await _context.Notes
            .Where(n => n.UserId == userId &&
                        ((n.Type == NoteType.Reminder && n.Reminder.Value.Date == today) ||
                         (n.Type == NoteType.Todo && n.DueDate.Value.Date == today)))
            .ToListAsync();
        return Ok(notes);
    }

    [HttpGet("dashboard/week")]
    [Authorize]
    public async Task<IActionResult> GetWeekNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Assuming Sunday as the start of the week
        var endOfWeek = startOfWeek.AddDays(7);

        var notes = await _context.Notes
            .Where(n => n.UserId == userId &&
                        ((n.Type == NoteType.Reminder && n.Reminder >= startOfWeek && n.Reminder < endOfWeek) ||
                         (n.Type == NoteType.Todo && n.DueDate >= startOfWeek && n.DueDate < endOfWeek)))
            .ToListAsync();

        return Ok(notes);
    }

    [HttpGet("dashboard/month")]
    [Authorize]
    public async Task<IActionResult> GetMonthNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var notes = await _context.Notes
            .Where(n => n.UserId == userId &&
                        ((n.Type == NoteType.Reminder && n.Reminder >= startOfMonth && n.Reminder < endOfMonth) ||
                         (n.Type == NoteType.Todo && n.DueDate >= startOfMonth && n.DueDate < endOfMonth)))
            .ToListAsync();

        return Ok(notes);
    }
}

