// Quiz state
let currentQuestions = [];
let currentQuestionIndex = 0;
let score = 0;
let answers = [];

// API endpoints
const API_BASE_URL = 'http://localhost:5188/api';

// DOM Elements
const welcomeScreen = document.getElementById('welcome-screen');
const quizScreen = document.getElementById('quiz-screen');
const resultScreen = document.getElementById('result-screen');
const categoriesContainer = document.getElementById('categories-container');
const categoryName = document.getElementById('category-name');
const questionText = document.getElementById('question-text');
const answersContainer = document.getElementById('answers-container');
const currentQuestionSpan = document.getElementById('current-question');
const totalQuestionsSpan = document.getElementById('total-questions');
const nextButton = document.getElementById('next-btn');
const finishButton = document.getElementById('finish-btn');
const restartButton = document.getElementById('restart-btn');
const scoreSpan = document.getElementById('score');
const totalScoreSpan = document.getElementById('total-score');
const resultBreakdown = document.getElementById('result-breakdown');

// Fetch categories and initialize the app
async function initializeApp() {
    try {
        const response = await fetch(`${API_BASE_URL}/Category`);
        const categories = await response.json();
        displayCategories(categories);
    } catch (error) {
        console.error('Error fetching categories:', error);
    }
}

// Display categories on the welcome screen
function displayCategories(categories) {
    categoriesContainer.innerHTML = categories.map(category => `
        <div class="category-card" onclick="startQuiz(${category.id}, '${category.name}')">
            <h3>${category.name}</h3>
            <p>${category.description || ''}</p>
        </div>
    `).join('');
}

// Start quiz for selected category
async function startQuiz(categoryId, categoryNameText) {
    try {
        const response = await fetch(`${API_BASE_URL}/Question/Category/${categoryId}`);
        currentQuestions = await response.json();
        
        if (currentQuestions.length === 0) {
            alert('No questions available for this category');
            return;
        }

        // Initialize quiz state
        currentQuestionIndex = 0;
        score = 0;
        answers = [];
        
        // Update UI
        categoryName.textContent = categoryNameText;
        totalQuestionsSpan.textContent = currentQuestions.length;
        
        // Show quiz screen
        welcomeScreen.classList.remove('active');
        quizScreen.classList.add('active');
        
        // Display first question
        displayQuestion();
    } catch (error) {
        console.error('Error fetching questions:', error);
    }
}

// Display current question and answers
function displayQuestion() {
    const question = currentQuestions[currentQuestionIndex];
    currentQuestionSpan.textContent = currentQuestionIndex + 1;
    questionText.textContent = question.text;
    
    // Display answers
    answersContainer.innerHTML = question.answers.map(answer => `
        <button class="answer-btn" onclick="selectAnswer(${answer.id}, ${answer.isCorrect})">
            ${answer.text}
        </button>
    `).join('');
    
    // Update buttons
    nextButton.style.display = 'none';
    finishButton.style.display = 'none';
}

// Handle answer selection
function selectAnswer(answerId, isCorrect) {
    // Disable all answer buttons
    const answerButtons = answersContainer.getElementsByClassName('answer-btn');
    Array.from(answerButtons).forEach(button => {
        button.disabled = true;
    });
    
    // Find selected button and mark correct/incorrect
    const selectedButton = Array.from(answerButtons).find(button => 
        button.textContent.trim() === currentQuestions[currentQuestionIndex].answers.find(a => a.id === answerId).text
    );
    
    if (isCorrect) {
        selectedButton.classList.add('correct');
        score++;
    } else {
        selectedButton.classList.add('incorrect');
        // Show correct answer
        const correctButton = Array.from(answerButtons).find(button => 
            button.textContent.trim() === currentQuestions[currentQuestionIndex].answers.find(a => a.isCorrect).text
        );
        correctButton.classList.add('correct');
    }
    
    // Store answer for result screen
    answers.push({
        question: currentQuestions[currentQuestionIndex].text,
        selectedAnswer: currentQuestions[currentQuestionIndex].answers.find(a => a.id === answerId).text,
        isCorrect: isCorrect
    });
    
    // Show next/finish button
    if (currentQuestionIndex < currentQuestions.length - 1) {
        nextButton.style.display = 'inline-block';
    } else {
        finishButton.style.display = 'inline-block';
    }
}

// Handle next question
function nextQuestion() {
    currentQuestionIndex++;
    displayQuestion();
}

// Show results
function showResults() {
    quizScreen.classList.remove('active');
    resultScreen.classList.add('active');
    
    scoreSpan.textContent = score;
    totalScoreSpan.textContent = currentQuestions.length;
    
    // Display result breakdown
    resultBreakdown.innerHTML = answers.map(answer => `
        <div class="result-item ${answer.isCorrect ? 'correct' : 'incorrect'}">
            <p><strong>Q: ${answer.question}</strong></p>
            <p>Your answer: ${answer.selectedAnswer}</p>
            <p>${answer.isCorrect ? '✓ Correct' : '✗ Incorrect'}</p>
        </div>
    `).join('');
}

// Event listeners
nextButton.addEventListener('click', nextQuestion);
finishButton.addEventListener('click', showResults);
restartButton.addEventListener('click', () => {
    resultScreen.classList.remove('active');
    welcomeScreen.classList.add('active');
});

// Initialize the app
initializeApp(); 