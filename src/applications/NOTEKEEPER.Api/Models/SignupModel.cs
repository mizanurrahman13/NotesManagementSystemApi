namespace NOTEKEEPER.Api.Models;

public class SignupModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Password { get; set; }
}
