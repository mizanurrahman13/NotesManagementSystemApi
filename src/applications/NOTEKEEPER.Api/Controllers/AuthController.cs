using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NOTEKEEPER.Api.Contexts;
using NOTEKEEPER.Api.Entities;
using NOTEKEEPER.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NOTEKEEPER.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AuthController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupModel model)
    {
        if (_context.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest("Email already in use");
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            DateOfBirth = model.DateOfBirth,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("9EE9E917-7A63-4409-B440-55F17517F70A-NoteKeeperNoteKeeperNoteKeeperNoteKeeperNoteKeeperNoteKeeperNoteKeeper");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "Issuer",
            Audience = "Audience"
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}



