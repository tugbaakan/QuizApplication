using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuizAPI.Data;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<QuizResult> QuizResults { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Countries", Description = "Questions about countries and geography" },
            new Category { Id = 2, Name = "Animals", Description = "Questions about animals and wildlife" },
            new Category { Id = 3, Name = "Programming", Description = "Questions about programming and software development" },
            new Category { Id = 4, Name = "Cyber Security", Description = "Questions about cyber security and information security" }
        );

        // Configure relationships
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Category)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.User)
            .WithMany(u => u.QuizResults)
            .HasForeignKey(qr => qr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.Category)
            .WithMany()
            .HasForeignKey(qr => qr.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "your_hashed_password_here", // You'll need to hash this
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 