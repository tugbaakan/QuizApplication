using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly QuizDbContext _context;

    public QuestionController(QuizDbContext context)
    {
        _context = context;
    }

    // GET: api/Question
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
    {
        return await _context.Questions
            .Include(q => q.Category)
            .Include(q => q.Answers)
            .ToListAsync();
    }

    // GET: api/Question/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Category)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            return NotFound();
        }

        return question;
    }

    // GET: api/Question/Category/5
    [HttpGet("Category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByCategory(int categoryId)
    {
        return await _context.Questions
            .Include(q => q.Category)
            .Include(q => q.Answers)
            .Where(q => q.CategoryId == categoryId)
            .ToListAsync();
    }

    // POST: api/Question
    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(Question question)
    {
        // Validate category exists
        if (!await _context.Categories.AnyAsync(c => c.Id == question.CategoryId))
        {
            return BadRequest("Invalid category ID");
        }

        question.CreatedAt = DateTime.UtcNow;
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
    }

    // PUT: api/Question/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, Question question)
    {
        if (id != question.Id)
        {
            return BadRequest();
        }

        var existingQuestion = await _context.Questions.FindAsync(id);
        if (existingQuestion == null)
        {
            return NotFound();
        }

        // Update only allowed fields
        existingQuestion.Text = question.Text;
        existingQuestion.CategoryId = question.CategoryId;
        existingQuestion.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuestionExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Question/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool QuestionExists(int id)
    {
        return _context.Questions.Any(e => e.Id == id);
    }
} 