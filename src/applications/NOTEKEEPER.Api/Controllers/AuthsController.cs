using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
public class AuthsController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly JwtSetting _jwtSetting;

    public AuthsController(ApplicationContext context, IOptions<JwtSetting> jwtSetting)
    {
        _context = context;
        _jwtSetting = jwtSetting.Value;
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
        var key = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
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
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}



