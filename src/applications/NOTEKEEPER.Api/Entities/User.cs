namespace NOTEKEEPER.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PasswordHash { get; set; } // Store hashed passwords for security
    public List<Note> Notes { get; set; }
}
