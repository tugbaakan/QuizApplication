using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models;

public class QuizResult
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;

    [Required]
    public int TotalQuestions { get; set; }

    [Required]
    public int CorrectAnswers { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
} 