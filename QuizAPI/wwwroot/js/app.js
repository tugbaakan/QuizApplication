console.log('app.js loaded');

document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM Content Loaded in app.js');
    // DOM Elements
    const userInfo = document.getElementById('userInfo');
    const authLinks = document.getElementById('authLinks');
    const authMessage = document.getElementById('authMessage');
    const categoryList = document.getElementById('categoryList');
    const quizContainer = document.getElementById('quizContainer');
    const resultsContainer = document.getElementById('resultsContainer');
    const usernameSpan = document.getElementById('username');
    const logoutBtn = document.getElementById('logoutBtn');

    // Quiz state
    let currentQuiz = {
        categoryId: null,
        questions: [],
        currentQuestionIndex: 0,
        correctAnswers: 0
    };

    // Check authentication state
    function checkAuth() {
        console.log('Checking authentication state...');
        const token = localStorage.getItem('token');
        const username = localStorage.getItem('username');
        
        console.log('Auth state:', {
            token: token ? 'Present' : 'Missing',
            username: username || 'Missing'
        });

        if (token && username) {
            console.log('User is logged in, showing user info');
            // User is logged in
            userInfo.style.display = 'inline';
            authLinks.style.display = 'none';
            usernameSpan.textContent = username;
            authMessage.style.display = 'none';
            categoryList.style.display = 'grid';
            loadCategories();
        } else {
            console.log('User is not logged in, showing auth links');
            // User is not logged in
            userInfo.style.display = 'none';
            authLinks.style.display = 'inline';
            authMessage.style.display = 'block';
            categoryList.style.display = 'none';
        }
    }

    // Load quiz categories
    async function loadCategories() {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch('/api/Category', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const categories = await response.json();
                displayCategories(categories);
            }
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    }

    // Display categories
    function displayCategories(categories) {
        categoryList.innerHTML = categories.map(category => `
            <div class="category-card" data-id="${category.id}">
                <h3>${category.name}</h3>
                <p>${category.description}</p>
            </div>
        `).join('');

        // Add click event listeners to category cards
        document.querySelectorAll('.category-card').forEach(card => {
            card.addEventListener('click', () => startQuiz(parseInt(card.dataset.id)));
        });
    }

    // Start quiz
    async function startQuiz(categoryId) {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`/api/Question/category/${categoryId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const questions = await response.json();
                currentQuiz = {
                    categoryId,
                    questions,
                    currentQuestionIndex: 0,
                    correctAnswers: 0
                };
                displayQuestion();
                categoryList.style.display = 'none';
                quizContainer.style.display = 'block';
            }
        } catch (error) {
            console.error('Error starting quiz:', error);
        }
    }

    // Display current question
    function displayQuestion() {
        const question = currentQuiz.questions[currentQuiz.currentQuestionIndex];
        quizContainer.innerHTML = `
            <div class="quiz-header">
                <div class="progress">Question ${currentQuiz.currentQuestionIndex + 1} of ${currentQuiz.questions.length}</div>
            </div>
            <div class="question">
                <p>${question.text}</p>
                <div class="answers">
                    ${question.answers.map(answer => `
                        <button class="answer-btn" data-correct="${answer.isCorrect}">
                            ${answer.text}
                        </button>
                    `).join('')}
                </div>
            </div>
        `;

        // Add click event listeners to answer buttons
        document.querySelectorAll('.answer-btn').forEach(button => {
            button.addEventListener('click', () => handleAnswer(button));
        });
    }

    // Handle answer selection
    async function handleAnswer(button) {
        const isCorrect = button.dataset.correct === 'true';
        if (isCorrect) {
            currentQuiz.correctAnswers++;
            button.classList.add('correct');
        } else {
            button.classList.add('incorrect');
            document.querySelector('[data-correct="true"]').classList.add('correct');
        }

        // Disable all buttons after answer
        document.querySelectorAll('.answer-btn').forEach(btn => btn.disabled = true);

        // Wait before moving to next question
        setTimeout(() => {
            if (currentQuiz.currentQuestionIndex < currentQuiz.questions.length - 1) {
                currentQuiz.currentQuestionIndex++;
                displayQuestion();
            } else {
                finishQuiz();
            }
        }, 1500);
    }

    // Finish quiz and save results
    async function finishQuiz() {
        const token = localStorage.getItem('token');
        try {
            const response = await fetch('/api/QuizResult', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    resultDto: {
                        categoryId: currentQuiz.categoryId,
                        totalQuestions: currentQuiz.questions.length,
                        correctAnswers: currentQuiz.correctAnswers
                    }
                })
            });

            if (response.ok) {
                displayResults();
            }
        } catch (error) {
            console.error('Error saving quiz results:', error);
        }
    }

    // Display quiz results
    function displayResults() {
        const score = (currentQuiz.correctAnswers / currentQuiz.questions.length) * 100;
        quizContainer.style.display = 'none';
        resultsContainer.style.display = 'block';
        resultsContainer.innerHTML = `
            <h2>Quiz Complete!</h2>
            <div class="results">
                <p>Your Score: ${currentQuiz.correctAnswers} out of ${currentQuiz.questions.length}</p>
                <p>Percentage: ${score.toFixed(1)}%</p>
                <button class="try-again-btn">Try Another Category</button>
            </div>
        `;

        document.querySelector('.try-again-btn').addEventListener('click', () => {
            resultsContainer.style.display = 'none';
            categoryList.style.display = 'grid';
        });
    }

    // Handle logout
    if (logoutBtn) {
        logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('userId');
            localStorage.removeItem('username');
            localStorage.removeItem('isAdmin');
            window.location.href = '/login.html';
        });
    }

    // Initialize
    checkAuth();
}); 