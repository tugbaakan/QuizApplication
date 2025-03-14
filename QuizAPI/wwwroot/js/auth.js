console.log('auth.js loaded');

document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM Content Loaded in auth.js');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const errorMessage = document.getElementById('errorMessage');

    // Handle login
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;

            try {
                console.log('Attempting login...');
                const response = await fetch('/api/Auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ username, password })
                });

                if (response.ok) {
                    const data = await response.json();
                    console.log('Login successful, received data:', data);
                    
                    // Store tokens and user info
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('refreshToken', data.refreshToken);
                    localStorage.setItem('userId', data.user.id);
                    localStorage.setItem('username', data.user.username);
                    localStorage.setItem('isAdmin', data.user.isAdmin);
                    
                    console.log('Stored in localStorage:', {
                        token: localStorage.getItem('token'),
                        refreshToken: localStorage.getItem('refreshToken'),
                        userId: localStorage.getItem('userId'),
                        username: localStorage.getItem('username'),
                        isAdmin: localStorage.getItem('isAdmin')
                    });

                    window.location.href = '/';
                } else {
                    const error = await response.text();
                    errorMessage.textContent = error;
                    /* console.error('Login failed:', error);
                    alert(error.message || 'Login failed');*/
                }
            } catch (error) {
                errorMessage.textContent = 'An error occurred. Please try again.';            
                console.error('Login error:', error);
                /*alert('An error occurred during login');*/
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
    const token = localStorage.getItem('token');
    if (token && (window.location.pathname === '/login.html' || window.location.pathname === '/register.html')) {
        window.location.href = '/';
    }
}); 