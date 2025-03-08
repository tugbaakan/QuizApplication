using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Data;

public static class SampleData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new QuizDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<QuizDbContext>>());

        // Check if we already have questions
        if (await context.Questions.AnyAsync())
        {
            return;   // Database has already been seeded
        }

        // Countries Category (ID: 1)
        var countryQuestions = new List<Question>
        {
            new Question
            {
                Text = "What is the capital of France?",
                CategoryId = 1,
                Answers = new List<Answer>
                {
                    new Answer { Text = "Paris", IsCorrect = true },
                    new Answer { Text = "London", IsCorrect = false },
                    new Answer { Text = "Berlin", IsCorrect = false },
                    new Answer { Text = "Madrid", IsCorrect = false }
                }
            },
            new Question
            {
                Text = "Which is the largest country by area?",
                CategoryId = 1,
                Answers = new List<Answer>
                {
                    new Answer { Text = "Russia", IsCorrect = true },
                    new Answer { Text = "China", IsCorrect = false },
                    new Answer { Text = "USA", IsCorrect = false },
                    new Answer { Text = "Canada", IsCorrect = false }
                }
            }
        };

        // Animals Category (ID: 2)
        var animalQuestions = new List<Question>
        {
            new Question
            {
                Text = "Which is the fastest land animal?",
                CategoryId = 2,
                Answers = new List<Answer>
                {
                    new Answer { Text = "Cheetah", IsCorrect = true },
                    new Answer { Text = "Lion", IsCorrect = false },
                    new Answer { Text = "Gazelle", IsCorrect = false },
                    new Answer { Text = "Horse", IsCorrect = false }
                }
            },
            new Question
            {
                Text = "What is the largest animal in the world?",
                CategoryId = 2,
                Answers = new List<Answer>
                {
                    new Answer { Text = "Blue Whale", IsCorrect = true },
                    new Answer { Text = "African Elephant", IsCorrect = false },
                    new Answer { Text = "Giraffe", IsCorrect = false },
                    new Answer { Text = "Colossal Squid", IsCorrect = false }
                }
            }
        };

        // Programming Category (ID: 3)
        var programmingQuestions = new List<Question>
        {
            new Question
            {
                Text = "What is inheritance in OOP?",
                CategoryId = 3,
                Answers = new List<Answer>
                {
                    new Answer { Text = "A mechanism that allows a class to inherit properties and methods from another class", IsCorrect = true },
                    new Answer { Text = "A way to create multiple instances of a class", IsCorrect = false },
                    new Answer { Text = "A method to store data in a database", IsCorrect = false },
                    new Answer { Text = "A technique to optimize code performance", IsCorrect = false }
                }
            },
            new Question
            {
                Text = "Which of these is not a programming paradigm?",
                CategoryId = 3,
                Answers = new List<Answer>
                {
                    new Answer { Text = "Waterfall", IsCorrect = true },
                    new Answer { Text = "Functional", IsCorrect = false },
                    new Answer { Text = "Object-Oriented", IsCorrect = false },
                    new Answer { Text = "Procedural", IsCorrect = false }
                }
            }
        };

        // Cyber Security Category (ID: 4)
        var securityQuestions = new List<Question>
        {
            new Question
            {
                Text = "What is SQL Injection?",
                CategoryId = 4,
                Answers = new List<Answer>
                {
                    new Answer { Text = "A code injection technique used to attack data-driven applications", IsCorrect = true },
                    new Answer { Text = "A database backup method", IsCorrect = false },
                    new Answer { Text = "A type of database optimization", IsCorrect = false },
                    new Answer { Text = "A SQL query builder tool", IsCorrect = false }
                }
            },
            new Question
            {
                Text = "What is the purpose of a firewall?",
                CategoryId = 4,
                Answers = new List<Answer>
                {
                    new Answer { Text = "To monitor and control incoming and outgoing network traffic", IsCorrect = true },
                    new Answer { Text = "To speed up internet connection", IsCorrect = false },
                    new Answer { Text = "To store website data locally", IsCorrect = false },
                    new Answer { Text = "To compress data files", IsCorrect = false }
                }
            }
        };

        // Add all questions to the context
        await context.Questions.AddRangeAsync(countryQuestions);
        await context.Questions.AddRangeAsync(animalQuestions);
        await context.Questions.AddRangeAsync(programmingQuestions);
        await context.Questions.AddRangeAsync(securityQuestions);

        // Save changes to the database
        await context.SaveChangesAsync();
    }
} 