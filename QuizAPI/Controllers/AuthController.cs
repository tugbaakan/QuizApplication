using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;
using QuizAPI.Services;
using System.Security.Cryptography;
using System.Text;

namespace QuizAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly QuizDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(QuizDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            return BadRequest("Username already exists");
        }

        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return BadRequest("Email already exists");
        }

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null || user.PasswordHash != HashPassword(loginDto.Password))
        {
            return Unauthorized("Invalid username or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate tokens
        var jwtToken = _jwtService.GenerateJwtToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user);

        // Save refresh token
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = jwtToken,
            refreshToken = refreshToken.Token,
            expiresIn = 15 * 60, // 15 minutes in seconds
            user = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.IsAdmin
            }
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if (refreshToken == null)
        {
            return BadRequest("Invalid refresh token");
        }

        if (refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return BadRequest("Refresh token has expired or been revoked");
        }

        // Revoke the old refresh token
        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        // Generate new tokens
        var newJwtToken = _jwtService.GenerateJwtToken(refreshToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken(refreshToken.User);

        // Save new refresh token
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = newJwtToken,
            refreshToken = newRefreshToken.Token,
            expiresIn = 15 * 60 // 15 minutes in seconds
        });
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if (refreshToken == null)
        {
            return BadRequest("Invalid refresh token");
        }

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Token revoked successfully" });
    }
}

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; }
} 