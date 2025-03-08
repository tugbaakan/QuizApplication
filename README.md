# Quiz Application API

A RESTful API built with .NET Core for managing a quiz application. The application supports multiple categories of questions, with features for both quiz-taking and administrative management.

## Features

- Multiple quiz categories (Countries, Animals, Programming, Cyber Security)
- RESTful API endpoints for managing questions and answers
- PostgreSQL database for data persistence
- Swagger UI for API documentation and testing
- Admin dashboard functionality for managing quiz content

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 15 or later
- An IDE (Visual Studio, VS Code, or JetBrains Rider)

## Getting Started

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd QuizApplication
```

### 2. Database Setup

1. Install PostgreSQL if you haven't already
2. Create a new database named 'quizdb'
3. Configure your connection string (see Configuration section below)

### 3. Configuration

1. Navigate to the QuizAPI directory
2. Copy the example configuration file:
   ```bash
   cp appsettings.example.json appsettings.json
   ```
3. Update `appsettings.json` with your PostgreSQL credentials
4. For development, use User Secrets to store your database connection string:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=quizdb;Username=your_username;Password=your_password"
   ```

### 4. Database Migrations

Run the following commands to create and update the database:

```bash
dotnet ef database update
```

### 5. Running the Application

```bash
dotnet run
```

The API will be available at:
- API: https://localhost:5188
- Swagger UI: https://localhost:5188/swagger

## API Endpoints

### Categories
- GET /api/Category - Get all categories
- GET /api/Category/{id} - Get a specific category
- POST /api/Category - Create a new category
- PUT /api/Category/{id} - Update a category
- DELETE /api/Category/{id} - Delete a category

### Questions
- GET /api/Question - Get all questions
- GET /api/Question/{id} - Get a specific question
- GET /api/Question/Category/{categoryId} - Get questions by category
- POST /api/Question - Create a new question
- PUT /api/Question/{id} - Update a question
- DELETE /api/Question/{id} - Delete a question

### Answers
- GET /api/Answer/Question/{questionId} - Get answers for a question
- GET /api/Answer/{id} - Get a specific answer
- POST /api/Answer - Create a new answer
- PUT /api/Answer/{id} - Update an answer
- DELETE /api/Answer/{id} - Delete an answer

## Project Structure

```
QuizAPI/
├── Controllers/         # API Controllers
├── Models/             # Domain Models
├── Data/               # Database Context and Configurations
├── Services/           # Business Logic
├── Migrations/         # Database Migrations
└── Properties/         # Launch Settings
```

## Security Considerations

- The application uses user secrets for storing sensitive configuration in development
- Production deployments should use environment variables or secure configuration management
- Database passwords and other sensitive data should never be committed to source control

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

[Your chosen license] 