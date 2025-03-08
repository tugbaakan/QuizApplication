document.addEventListener('DOMContentLoaded', () => {
    const userInfo = document.getElementById('userInfo');
    const authLinks = document.getElementById('authLinks');
    const authMessage = document.getElementById('authMessage');
    const historyContainer = document.getElementById('historyContainer');
    const usernameSpan = document.getElementById('username');
    const logoutBtn = document.getElementById('logoutBtn');
    const categoryFilter = document.getElementById('categoryFilter');
    const dateFilter = document.getElementById('dateFilter');
    const quizList = document.getElementById('quizList');
    const totalQuizzes = document.getElementById('totalQuizzes');
    const averageScore = document.getElementById('averageScore');
    const bestCategory = document.getElementById('bestCategory');

    let currentUser = null;
    let quizResults = [];
    let categories = [];

    // Check authentication status
    function checkAuth() {
        const userId = localStorage.getItem('userId');
        const username = localStorage.getItem('username');
        
        if (userId && username) {
            currentUser = {
                id: userId,
                username: username,
                isAdmin: localStorage.getItem('isAdmin') === 'true'
            };
            
            userInfo.style.display = 'block';
            authLinks.style.display = 'none';
            authMessage.style.display = 'none';
            historyContainer.style.display = 'block';
            usernameSpan.textContent = username;
            
            loadCategories();
            loadQuizHistory();
        } else {
            userInfo.style.display = 'none';
            authLinks.style.display = 'block';
            authMessage.style.display = 'block';
            historyContainer.style.display = 'none';
        }
    }

    // Load categories for filter
    async function loadCategories() {
        try {
            const response = await fetch('/api/Category');
            if (response.ok) {
                categories = await response.json();
                categoryFilter.innerHTML = `
                    <option value="">All Categories</option>
                    ${categories.map(category => `
                        <option value="${category.id}">${category.name}</option>
                    `).join('')}
                `;
            }
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    }

    // Load quiz history
    async function loadQuizHistory() {
        try {
            const response = await fetch(`/api/QuizResult/user/${currentUser.id}`);
            if (response.ok) {
                quizResults = await response.json();
                filterAndDisplayResults();
            }
        } catch (error) {
            console.error('Error loading quiz history:', error);
        }
    }

    // Filter and display results
    function filterAndDisplayResults() {
        const selectedCategory = categoryFilter.value;
        const selectedDate = dateFilter.value;
        
        let filteredResults = [...quizResults];

        // Apply category filter
        if (selectedCategory) {
            filteredResults = filteredResults.filter(result => 
                result.categoryId === parseInt(selectedCategory)
            );
        }

        // Apply date filter
        const now = new Date();
        switch (selectedDate) {
            case 'week':
                const lastWeek = new Date(now.setDate(now.getDate() - 7));
                filteredResults = filteredResults.filter(result => 
                    new Date(result.completedAt) > lastWeek
                );
                break;
            case 'month':
                const lastMonth = new Date(now.setMonth(now.getMonth() - 1));
                filteredResults = filteredResults.filter(result => 
                    new Date(result.completedAt) > lastMonth
                );
                break;
            case 'year':
                const lastYear = new Date(now.setFullYear(now.getFullYear() - 1));
                filteredResults = filteredResults.filter(result => 
                    new Date(result.completedAt) > lastYear
                );
                break;
        }

        // Update statistics
        updateStatistics(filteredResults);

        // Display results
        quizList.innerHTML = filteredResults.map(result => `
            <div class="quiz-card">
                <div class="quiz-info">
                    <h3>${result.categoryName}</h3>
                    <p>${result.correctAnswers} correct answers out of ${result.totalQuestions} questions</p>
                </div>
                <div class="quiz-score">
                    <div class="score">${result.score.toFixed(1)}%</div>
                    <div class="total">Score</div>
                </div>
                <div class="quiz-date">
                    ${new Date(result.completedAt).toLocaleDateString()}
                </div>
            </div>
        `).join('');
    }

    // Update statistics
    function updateStatistics(results) {
        // Total quizzes
        totalQuizzes.textContent = results.length;

        // Average score
        const avgScore = results.length > 0
            ? results.reduce((sum, result) => sum + result.score, 0) / results.length
            : 0;
        averageScore.textContent = `${avgScore.toFixed(1)}%`;

        // Best category
        if (results.length > 0) {
            const categoryScores = {};
            results.forEach(result => {
                if (!categoryScores[result.categoryName]) {
                    categoryScores[result.categoryName] = {
                        totalScore: 0,
                        count: 0
                    };
                }
                categoryScores[result.categoryName].totalScore += result.score;
                categoryScores[result.categoryName].count++;
            });

            let bestCategoryName = '-';
            let bestAverage = 0;
            for (const [category, scores] of Object.entries(categoryScores)) {
                const average = scores.totalScore / scores.count;
                if (average > bestAverage) {
                    bestAverage = average;
                    bestCategoryName = category;
                }
            }
            bestCategory.textContent = bestCategoryName;
        } else {
            bestCategory.textContent = '-';
        }
    }

    // Event listeners for filters
    categoryFilter.addEventListener('change', filterAndDisplayResults);
    dateFilter.addEventListener('change', filterAndDisplayResults);

    // Handle logout
    if (logoutBtn) {
        logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.removeItem('userId');
            localStorage.removeItem('username');
            localStorage.removeItem('isAdmin');
            window.location.reload();
        });
    }

    // Initialize
    checkAuth();
}); 