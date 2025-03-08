using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswerController : ControllerBase
{
    private readonly QuizDbContext _context;

    public AnswerController(QuizDbContext context)
    {
        _context = context;
    }

    // GET: api/Answer/Question/5
    [HttpGet("Question/{questionId}")]
    public async Task<ActionResult<IEnumerable<Answer>>> GetAnswersByQuestion(int questionId)
    {
        return await _context.Answers
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
    }

    // GET: api/Answer/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Answer>> GetAnswer(int id)
    {
        var answer = await _context.Answers
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (answer == null)
        {
            return NotFound();
        }

        return answer;
    }

    // POST: api/Answer
    [HttpPost]
    public async Task<ActionResult<Answer>> CreateAnswer(Answer answer)
    {
        // Validate question exists
        if (!await _context.Questions.AnyAsync(q => q.Id == answer.QuestionId))
        {
            return BadRequest("Invalid question ID");
        }

        // Ensure only one correct answer per question
        if (answer.IsCorrect)
        {
            var existingCorrectAnswer = await _context.Answers
                .AnyAsync(a => a.QuestionId == answer.QuestionId && a.IsCorrect);
            
            if (existingCorrectAnswer)
            {
                return BadRequest("A correct answer already exists for this question");
            }
        }

        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
    }

    // PUT: api/Answer/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnswer(int id, Answer answer)
    {
        if (id != answer.Id)
        {
            return BadRequest();
        }

        var existingAnswer = await _context.Answers.FindAsync(id);
        if (existingAnswer == null)
        {
            return NotFound();
        }

        // Check for correct answer conflict
        if (answer.IsCorrect && answer.QuestionId == existingAnswer.QuestionId)
        {
            var hasAnotherCorrectAnswer = await _context.Answers
                .AnyAsync(a => a.QuestionId == answer.QuestionId 
                              && a.Id != id 
                              && a.IsCorrect);
            
            if (hasAnotherCorrectAnswer)
            {
                return BadRequest("Another correct answer already exists for this question");
            }
        }

        existingAnswer.Text = answer.Text;
        existingAnswer.IsCorrect = answer.IsCorrect;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnswerExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Answer/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer == null)
        {
            return NotFound();
        }

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AnswerExists(int id)
    {
        return _context.Answers.Any(e => e.Id == id);
    }
} 