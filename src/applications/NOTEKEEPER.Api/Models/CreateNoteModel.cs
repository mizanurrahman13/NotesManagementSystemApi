using NOTEKEEPER.Api.Enums;

namespace NOTEKEEPER.Api.Models;

public class CreateNoteModel
{
    public string Text { get; set; }
    public NoteType Type { get; set; }
    public DateTime? Reminder { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsComplete { get; set; }
    public string Url { get; set; }
}
