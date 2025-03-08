using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuizAPI.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    // Navigation property
    [JsonIgnore]
    public ICollection<Question> Questions { get; set; } = new List<Question>();
} 