// Toast Notification System
class Toast {
    static container = null;
    static init() {
        if (!Toast.container) {
            Toast.container = document.createElement('div');
            Toast.container.className = 'toast-container';
            document.body.appendChild(Toast.container);
        }
    }

    static show(message, type = 'info', duration = 5000) {
        Toast.init();

        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;

        const icons = {
            success: '✓',
            error: '✗',
            warning: '⚠',
            info: 'ⓘ'
        };

        toast.innerHTML = `
            <div class="toast-icon">${icons[type] || icons.info}</div>
            <div class="toast-content">${message}</div>
            <button class="toast-close" aria-label="Close notification">×</button>
        `;

        Toast.container.appendChild(toast);

        // Trigger animation
        setTimeout(() => toast.classList.add('show'), 10);

        // Auto remove
        const removeToast = () => {
            toast.classList.remove('show');
            toast.classList.add('hide');
            setTimeout(() => {
                if (toast.parentNode === Toast.container) {
                    Toast.container.removeChild(toast);
                }
            }, 300);
        };

        // Close button
        toast.querySelector('.toast-close').addEventListener('click', removeToast);

        // Auto remove after duration
        if (duration > 0) {
            setTimeout(removeToast, duration);
        }

        return {
            dismiss: removeToast
        };
    }

    static success(message, duration) {
        return Toast.show(message, 'success', duration);
    }

    static error(message, duration) {
        return Toast.show(message, 'error', duration);
    }

    static warning(message, duration) {
        return Toast.show(message, 'warning', duration);
    }

    static info(message, duration) {
        return Toast.show(message, 'info', duration);
    }
}

// Form Validation and Enhancement
class AuthFormHandler {
    constructor(formId) {
        this.form = document.getElementById(formId);
        if (!this.form) return;

        this.init();
    }

    init() {
        this.setupPasswordStrength();
        this.setupInputValidation();
        this.setupSubmitHandler();
        this.setupAnimations();
        this.checkSessionData();
    }

    setupPasswordStrength() {
        const passwordInput = this.form.querySelector('input[type="password"]');
        if (!passwordInput) return;

        const strengthMeter = document.createElement('div');
        strengthMeter.className = 'password-strength';
        strengthMeter.innerHTML = '<div class="strength-meter"></div>';

        passwordInput.parentNode.appendChild(strengthMeter);
        const meter = strengthMeter.querySelector('.strength-meter');

        passwordInput.addEventListener('input', (e) => {
            const strength = this.calculatePasswordStrength(e.target.value);
            meter.className = 'strength-meter';
            meter.classList.add(`strength-${strength.level}`);
            meter.style.width = `${strength.score * 25}%`;
        });
    }

    calculatePasswordStrength(password) {
        let score = 0;

        // Length check
        if (password.length >= 8) score++;
        if (password.length >= 12) score++;

        // Complexity checks
        if (/[A-Z]/.test(password)) score++;
        if (/[a-z]/.test(password)) score++;
        if (/[0-9]/.test(password)) score++;
        if (/[^A-Za-z0-9]/.test(password)) score++;

        // Cap at 4 for our meter
        score = Math.min(score, 4);

        const levels = ['weak', 'fair', 'good', 'strong'];
        return {
            score,
            level: levels[score - 1] || 'weak'
        };
    }

    setupInputValidation() {
        const inputs = this.form.querySelectorAll('input');
        inputs.forEach(input => {
            // Real-time validation
            input.addEventListener('blur', (e) => {
                this.validateField(e.target);
            });

            // Clear error on focus
            input.addEventListener('focus', (e) => {
                this.clearFieldError(e.target);
            });

            // Email validation
            if (input.type === 'email') {
                input.addEventListener('input', (e) => {
                    if (this.isValidEmail(e.target.value)) {
                        this.showFieldSuccess(e.target, 'Valid email format');
                    }
                });
            }
        });
    }

    validateField(field) {
        const value = field.value.trim();
        let isValid = true;
        let message = '';

        // Required field check
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            message = 'This field is required';
        }

        // Email validation
        if (field.type === 'email' && value && !this.isValidEmail(value)) {
            isValid = false;
            message = 'Please enter a valid email address';
        }

        // Password validation
        if (field.type === 'password' && value) {
            if (value.length < 8) {
                isValid = false;
                message = 'Password must be at least 8 characters';
            }
        }

        // Date validation
        if (field.type === 'date' && value) {
            const date = new Date(value);
            const now = new Date();
            if (date > now) {
                isValid = false;
                message = 'Date cannot be in the future';
            }
        }

        if (!isValid) {
            this.showFieldError(field, message);
        } else if (value) {
            this.showFieldSuccess(field);
        }

        return isValid;
    }

    isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    showFieldError(field, message) {
        this.clearFieldError(field);
        field.classList.add('error');

        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.innerHTML = `<i class="fas fa-exclamation-circle"></i> ${message}`;

        field.parentNode.appendChild(errorDiv);
    }

    showFieldSuccess(field, message = 'Looks good!') {
        this.clearFieldError(field);

        const successDiv = document.createElement('div');
        successDiv.className = 'success-message';
        successDiv.innerHTML = `<i class="fas fa-check-circle"></i> ${message}`;
        successDiv.style.color = '#10b981';
        successDiv.style.fontSize = '0.875rem';
        successDiv.style.marginTop = '0.25rem';

        field.parentNode.appendChild(successDiv);

        // Remove success message after 3 seconds
        setTimeout(() => {
            if (successDiv.parentNode) {
                successDiv.parentNode.removeChild(successDiv);
            }
        }, 3000);
    }

    clearFieldError(field) {
        field.classList.remove('error');

        const existingError = field.parentNode.querySelector('.error-message');
        if (existingError) {
            existingError.parentNode.removeChild(existingError);
        }

        const existingSuccess = field.parentNode.querySelector('.success-message');
        if (existingSuccess) {
            existingSuccess.parentNode.removeChild(existingSuccess);
        }
    }

    setupSubmitHandler() {
        this.form.addEventListener('submit', async (e) => {
            e.preventDefault();

            // Validate all fields
            const inputs = this.form.querySelectorAll('input[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!this.validateField(input)) {
                    isValid = false;
                }
            });

            if (!isValid) {
                Toast.error('Please fix the errors in the form');
                return;
            }

            // Show loading state
            const submitBtn = this.form.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            submitBtn.classList.add('btn-loading');
            submitBtn.disabled = true;

            try {
                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 1500));

                // Submit the form
                this.form.submit();
            } catch (error) {
                submitBtn.classList.remove('btn-loading');
                submitBtn.innerHTML = originalText;
                submitBtn.disabled = false;
                Toast.error('An error occurred. Please try again.');
            }
        });
    }

    setupAnimations() {
        // Add subtle hover animations
        const inputs = this.form.querySelectorAll('input, button, a');
        inputs.forEach(element => {
            element.addEventListener('mouseenter', () => {
                element.style.transform = 'translateY(-1px)';
            });

            element.addEventListener('mouseleave', () => {
                element.style.transform = 'translateY(0)';
            });
        });

        // Staggered input appearance
        const formGroups = this.form.querySelectorAll('.form-group');
        formGroups.forEach((group, index) => {
            group.style.opacity = '0';
            group.style.transform = 'translateY(10px)';

            setTimeout(() => {
                group.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                group.style.opacity = '1';
                group.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }

    checkSessionData() {
        // Check for session expiration
        const lastActivity = sessionStorage.getItem('lastActivity');
        if (lastActivity) {
            const now = Date.now();
            const inactivityPeriod = now - parseInt(lastActivity);

            if (inactivityPeriod > 30 * 60 * 1000) { // 30 minutes
                Toast.warning('Your session has expired. Please sign in again.');
                setTimeout(() => {
                    window.location.href = '/Auth/Login';
                }, 2000);
            }
        }

        // Update last activity time
        document.addEventListener('click', () => {
            sessionStorage.setItem('lastActivity', Date.now().toString());
        });
    }
}

// OTP Input Handler
class OTPHandler {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        if (!this.container) return;

        this.init();
    }

    init() {
        const inputs = this.container.querySelectorAll('.otp-input');

        inputs.forEach((input, index) => {
            // Allow only numbers
            input.addEventListener('keydown', (e) => {
                if (!/^\d$/.test(e.key) &&
                    !['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
                    e.preventDefault();
                }
            });

            // Auto-focus next input
            input.addEventListener('input', (e) => {
                if (e.target.value.length === 1) {
                    if (index < inputs.length - 1) {
                        inputs[index + 1].focus();
                    }
                    e.target.classList.add('filled');
                }
            });

            // Handle backspace
            input.addEventListener('keydown', (e) => {
                if (e.key === 'Backspace' && !e.target.value && index > 0) {
                    inputs[index - 1].focus();
                    inputs[index - 1].classList.remove('filled');
                }
            });

            // Paste OTP from clipboard
            input.addEventListener('paste', (e) => {
                e.preventDefault();
                const pasteData = e.clipboardData.getData('text').trim();

                if (/^\d{6}$/.test(pasteData)) {
                    pasteData.split('').forEach((char, idx) => {
                        if (inputs[idx]) {
                            inputs[idx].value = char;
                            inputs[idx].classList.add('filled');
                        }
                    });
                    inputs[inputs.length - 1].focus();
                } else {
                    Toast.error('Please paste a valid 6-digit OTP code');
                }
            });
        });

        // Focus first input
        inputs[0].focus();
    }
}

// Theme Toggle
class ThemeManager {
    constructor() {
        this.init();
    }

    init() {
        this.currentTheme = localStorage.getItem('theme') || 'light';
        this.applyTheme();
        this.setupToggle();
    }

    applyTheme() {
        document.documentElement.setAttribute('data-theme', this.currentTheme);
        localStorage.setItem('theme', this.currentTheme);
    }

    setupToggle() {
        const toggleBtn = document.getElementById('themeToggle');
        if (!toggleBtn) return;

        toggleBtn.innerHTML = this.currentTheme === 'dark'
            ? '<i class="fas fa-sun"></i>'
            : '<i class="fas fa-moon"></i>';

        toggleBtn.addEventListener('click', () => {
            this.currentTheme = this.currentTheme === 'light' ? 'dark' : 'light';
            this.applyTheme();

            // Animate toggle
            toggleBtn.style.transform = 'scale(0.9)';
            setTimeout(() => {
                toggleBtn.style.transform = 'scale(1)';
                toggleBtn.innerHTML = this.currentTheme === 'dark'
                    ? '<i class="fas fa-sun"></i>'
                    : '<i class="fas fa-moon"></i>';
            }, 150);

            Toast.info(`Switched to ${this.currentTheme} mode`);
        });
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Initialize Toast system
    Toast.init();

    // Check for server-side toast messages
    const toastMessage = document.querySelector('[data-toast-message]');
    const toastType = document.querySelector('[data-toast-type]');

    if (toastMessage) {
        Toast.show(toastMessage.textContent, toastType?.textContent || 'info');
    }

    // Initialize form handlers
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        new AuthFormHandler('loginForm');
    }

    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        new AuthFormHandler('registerForm');
    }

    // Initialize OTP handler
    const otpContainer = document.querySelector('.otp-container');
    if (otpContainer) {
        new OTPHandler(otpContainer.id || 'otp-container');
    }

    // Initialize theme manager
    new ThemeManager();

    // Add interactive hover effects to buttons
    document.querySelectorAll('.btn').forEach(btn => {
        btn.addEventListener('mouseenter', () => {
            btn.style.transform = 'translateY(-2px)';
        });

        btn.addEventListener('mouseleave', () => {
            btn.style.transform = 'translateY(0)';
        });

        btn.addEventListener('mousedown', () => {
            btn.style.transform = 'scale(0.98)';
        });

        btn.addEventListener('mouseup', () => {
            btn.style.transform = 'scale(1)';
        });
    });

    // Password visibility toggle
    document.querySelectorAll('.password-toggle').forEach(toggle => {
        toggle.addEventListener('click', (e) => {
            const input = e.target.closest('.form-group').querySelector('input[type="password"]');
            if (input) {
                const type = input.type === 'password' ? 'text' : 'password';
                input.type = type;
                e.target.classList.toggle('fa-eye');
                e.target.classList.toggle('fa-eye-slash');
            }
        });
    });

    // Auto-capitalize names
    document.querySelectorAll('input[name*="name"], input[name*="Name"]').forEach(input => {
        input.addEventListener('blur', () => {
            if (input.value) {
                input.value = input.value.split(' ')
                    .map(word => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
                    .join(' ');
            }
        });
    });

    // Format phone number
    const phoneInput = document.querySelector('input[type="tel"]');
    if (phoneInput) {
        phoneInput.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\D/g, '');
            if (value.length > 10) value = value.slice(0, 10);

            if (value.length > 6) {
                value = `(${value.slice(0, 3)}) ${value.slice(3, 6)}-${value.slice(6)}`;
            } else if (value.length > 3) {
                value = `(${value.slice(0, 3)}) ${value.slice(3)}`;
            } else if (value.length > 0) {
                value = `(${value}`;
            }

            e.target.value = value;
        });
    }

    // Remember form data
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        const formId = form.id || 'auth-form';

        // Load saved data
        const savedData = localStorage.getItem(formId);
        if (savedData) {
            try {
                const data = JSON.parse(savedData);
                Object.keys(data).forEach(key => {
                    const input = form.querySelector(`[name="${key}"]`);
                    if (input && input.type !== 'password') {
                        input.value = data[key];
                    }
                });
            } catch (e) {
                console.error('Failed to load saved form data:', e);
            }
        }

        // Save data on input
        form.addEventListener('input', (e) => {
            if (e.target.name && e.target.type !== 'password') {
                const formData = {};
                new FormData(form).forEach((value, key) => {
                    if (key && value && !key.includes('password')) {
                        formData[key] = value;
                    }
                });
                localStorage.setItem(formId, JSON.stringify(formData));
            }
        });

        // Clear saved data on successful submit
        form.addEventListener('submit', () => {
            setTimeout(() => {
                localStorage.removeItem(formId);
            }, 1000);
        });
    });

    // Network status detection
    window.addEventListener('online', () => {
        Toast.success('You are back online!');
    });

    window.addEventListener('offline', () => {
        Toast.error('You are offline. Some features may not work.');
    });

    // Add loading animation to page transitions
    const links = document.querySelectorAll('a[href]:not([href^="#"]):not([href^="javascript"])');
    links.forEach(link => {
        link.addEventListener('click', (e) => {
            if (link.href && !link.target && link.href.startsWith(window.location.origin)) {
                e.preventDefault();

                // Show loading overlay
                const overlay = document.createElement('div');
                overlay.style.cssText = `
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: rgba(15, 23, 42, 0.8);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    z-index: 99999;
                    backdrop-filter: blur(4px);
                `;

                const spinner = document.createElement('div');
                spinner.style.cssText = `
                    width: 50px;
                    height: 50px;
                    border: 3px solid rgba(255, 255, 255, 0.3);
                    border-radius: 50%;
                    border-top-color: #fff;
                    animation: spin 1s linear infinite;
                `;

                overlay.appendChild(spinner);
                document.body.appendChild(overlay);

                setTimeout(() => {
                    window.location.href = link.href;
                }, 300);
            }
        });
    });
});

// Export for module usage if needed
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { Toast, AuthFormHandler, OTPHandler, ThemeManager };
}