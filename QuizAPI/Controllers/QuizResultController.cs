using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace QuizAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("strict")]
public class QuizResultController : ControllerBase
{
    private readonly QuizDbContext _context;

    public QuizResultController(QuizDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    [EnableRateLimiting("strict")]
    public async Task<ActionResult<IEnumerable<QuizResultDto>>> GetUserResults(int userId)
    {
        var results = await _context.QuizResults
            .Include(qr => qr.Category)
            .Where(qr => qr.UserId == userId)
            .OrderByDescending(qr => qr.CompletedAt)
            .Select(qr => new QuizResultDto
            {
                CategoryName = qr.Category.Name,
                TotalQuestions = qr.TotalQuestions,
                CorrectAnswers = qr.CorrectAnswers,
                CompletedAt = qr.CompletedAt,
                Score = (double)qr.CorrectAnswers / qr.TotalQuestions * 100
            })
            .ToListAsync();

        return Ok(results);
    }

    [HttpPost]
    [EnableRateLimiting("strict")]
    public async Task<ActionResult<QuizResult>> SaveResult([FromBody] SaveQuizResultRequest request)
    {
        // Get user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User not authenticated");
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid user ID in token");
        }

        var quizResult = new QuizResult
        {
            UserId = userId,
            CategoryId = request.resultDto.CategoryId,
            TotalQuestions = request.resultDto.TotalQuestions,
            CorrectAnswers = request.resultDto.CorrectAnswers,
            CompletedAt = DateTime.UtcNow
        };

        _context.QuizResults.Add(quizResult);
        await _context.SaveChangesAsync();

        return Ok(quizResult);
    }
}

public class QuizResultDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public DateTime CompletedAt { get; set; }
    public double Score { get; set; }
}

public class SaveQuizResultRequest
{
    public SaveQuizResultDto resultDto { get; set; } = null!;
}

public class SaveQuizResultDto
{
    public int CategoryId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
} 