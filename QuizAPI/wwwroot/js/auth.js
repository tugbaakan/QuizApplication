document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const errorMessage = document.getElementById('errorMessage');

    // Handle login
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = {
                username: document.getElementById('username').value,
                password: document.getElementById('password').value
            };

            try {
                const response = await fetch('/api/Auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    const userData = await response.json();
                    // Store user data in localStorage
                    localStorage.setItem('userId', userData.id);
                    localStorage.setItem('username', userData.username);
                    localStorage.setItem('isAdmin', userData.isAdmin);
                    
                    // Redirect to home page
                    window.location.href = '/';
                } else {
                    const error = await response.text();
                    errorMessage.textContent = error;
                }
            } catch (error) {
                errorMessage.textContent = 'An error occurred. Please try again.';
                console.error('Login error:', error);
            }
        });
    }

    // Handle registration
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = {
                username: document.getElementById('username').value,
                email: document.getElementById('email').value,
                password: document.getElementById('password').value
            };

            try {
                const response = await fetch('/api/Auth/register', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    // Redirect to login page after successful registration
                    window.location.href = '/login.html';
                } else {
                    const error = await response.text();
                    errorMessage.textContent = error;
                }
            } catch (error) {
                errorMessage.textContent = 'An error occurred. Please try again.';
                console.error('Registration error:', error);
            }
        });
    }

    // Check if user is already logged in
    const userId = localStorage.getItem('userId');
    if (userId && (window.location.pathname === '/login.html' || window.location.pathname === '/register.html')) {
        window.location.href = '/';
    }
}); 