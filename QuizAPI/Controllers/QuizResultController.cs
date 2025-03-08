using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizResultController : ControllerBase
{
    private readonly QuizDbContext _context;

    public QuizResultController(QuizDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
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
    public async Task<ActionResult<QuizResult>> SaveResult(SaveQuizResultDto resultDto)
    {
        var quizResult = new QuizResult
        {
            UserId = resultDto.UserId,
            CategoryId = resultDto.CategoryId,
            TotalQuestions = resultDto.TotalQuestions,
            CorrectAnswers = resultDto.CorrectAnswers,
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

public class SaveQuizResultDto
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
} 