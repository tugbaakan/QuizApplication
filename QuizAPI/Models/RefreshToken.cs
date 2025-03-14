using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Token { get; set; }
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    [Required]
    public bool IsRevoked { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
} 