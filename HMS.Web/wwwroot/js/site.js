/**
 * Medixus HMS - Main Layout JavaScript
 * Handles sidebar, theme, user dropdown, and navigation
 */

(function () {
    'use strict';

    // ============================================
    // DOM Elements
    // ============================================
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const themeToggle = document.getElementById('themeToggle');
    const userProfileToggle = document.getElementById('userProfileToggle');
    const userDropdown = document.getElementById('userDropdown');

    // ============================================
    // Sidebar Toggle Functionality
    // ============================================
    function toggleSidebar() {
        if (sidebar) {
            sidebar.classList.toggle('active');

            if (sidebarOverlay) {
                sidebarOverlay.classList.toggle('show');
            }

            // Update aria-expanded
            if (sidebarToggle) {
                const isExpanded = sidebar.classList.contains('active');
                sidebarToggle.setAttribute('aria-expanded', isExpanded);
            }
        }
    }

    function closeSidebar() {
        if (sidebar) {
            sidebar.classList.remove('active');
        }
        if (sidebarOverlay) {
            sidebarOverlay.classList.remove('show');
        }
        if (sidebarToggle) {
            sidebarToggle.setAttribute('aria-expanded', 'false');
        }
    }

    // Sidebar toggle button click
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            toggleSidebar();
        });
    }

    // Overlay click to close sidebar
    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', closeSidebar);
    }

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (e) {
        if (window.innerWidth <= 1024) {
            if (sidebar && sidebar.classList.contains('active')) {
                if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                    closeSidebar();
                }
            }
        }
    });

    // ============================================
    // Active Navigation Highlighting
    // ============================================
    function setActiveNavLink() {
        const navLinks = document.querySelectorAll('.nav-link');
        const currentPath = window.location.pathname.toLowerCase();

        navLinks.forEach(link => {
            const linkPath = new URL(link.href).pathname.toLowerCase();
            const navItem = link.closest('.nav-item');

            if (navItem) {
                // Check for exact match or if current path starts with link path
                if (linkPath === currentPath ||
                    (linkPath !== '/' && currentPath.startsWith(linkPath))) {
                    navItem.classList.add('active');
                } else {
                    navItem.classList.remove('active');
                }
            }
        });
    }

    // Set active nav on page load
    setActiveNavLink();

    // Update active nav on link click
    document.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function () {
            // Remove active from all
            document.querySelectorAll('.nav-item').forEach(item => {
                item.classList.remove('active');
            });
            // Add active to clicked
            const navItem = this.closest('.nav-item');
            if (navItem) {
                navItem.classList.add('active');
            }

            // Close sidebar on mobile after navigation
            if (window.innerWidth <= 1024) {
                closeSidebar();
            }
        });
    });

    // ============================================
    // Theme Toggle Functionality
    // ============================================
    function initTheme() {
        // Check for saved theme preference or default to light mode
        const savedTheme = localStorage.getItem('hms-theme');
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const currentTheme = savedTheme || (prefersDark ? 'dark' : 'light');

        // Apply theme
        document.documentElement.setAttribute('data-theme', currentTheme);

        // Update icon
        updateThemeIcon(currentTheme);
    }

    function updateThemeIcon(theme) {
        if (themeToggle) {
            const icon = themeToggle.querySelector('i');
            if (icon) {
                icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
            }
        }
    }

    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

        // Apply new theme
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('hms-theme', newTheme);

        // Update icon
        updateThemeIcon(newTheme);
    }

    // Initialize theme on page load
    initTheme();

    // Theme toggle button click
    if (themeToggle) {
        themeToggle.addEventListener('click', toggleTheme);
    }

    // ============================================
    // User Dropdown Functionality
    // ============================================
    function toggleUserDropdown(e) {
        if (e) {
            e.stopPropagation();
        }

        if (userDropdown && userProfileToggle) {
            const isShowing = userDropdown.classList.contains('show');

            if (isShowing) {
                closeUserDropdown();
            } else {
                userDropdown.classList.add('show');
                userProfileToggle.classList.add('active');
                userProfileToggle.setAttribute('aria-expanded', 'true');
            }
        }
    }

    function closeUserDropdown() {
        if (userDropdown && userProfileToggle) {
            userDropdown.classList.remove('show');
            userProfileToggle.classList.remove('active');
            userProfileToggle.setAttribute('aria-expanded', 'false');
        }
    }

    // User profile toggle click
    if (userProfileToggle) {
        userProfileToggle.addEventListener('click', toggleUserDropdown);
    }

    // Close dropdown when clicking outside
    document.addEventListener('click', function (e) {
        if (userDropdown && userProfileToggle) {
            if (!userDropdown.contains(e.target) && !userProfileToggle.contains(e.target)) {
                closeUserDropdown();
            }
        }
    });

    // ============================================
    // Keyboard Navigation
    // ============================================
    document.addEventListener('keydown', function (e) {
        // ESC key
        if (e.key === 'Escape') {
            closeSidebar();
            closeUserDropdown();
        }

        // Tab key trap in sidebar when open
        if (e.key === 'Tab' && sidebar && sidebar.classList.contains('active')) {
            const focusableElements = sidebar.querySelectorAll(
                'a[href], button:not([disabled])'
            );

            if (focusableElements.length > 0) {
                const firstFocusable = focusableElements[0];
                const lastFocusable = focusableElements[focusableElements.length - 1];

                if (e.shiftKey) {
                    if (document.activeElement === firstFocusable) {
                        lastFocusable.focus();
                        e.preventDefault();
                    }
                } else {
                    if (document.activeElement === lastFocusable) {
                        firstFocusable.focus();
                        e.preventDefault();
                    }
                }
            }
        }
    });

    // ============================================
    // Responsive Behavior
    // ============================================
    let resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            // Close sidebar on desktop
            if (window.innerWidth > 1024) {
                closeSidebar();
            }

            // Close user dropdown on resize
            closeUserDropdown();
        }, 250);
    });

    // ============================================
    // Smooth Scroll for Anchor Links
    // ============================================
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href && href !== '#' && href.length > 1) {
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // ============================================
    // Page Load Animation
    // ============================================
    window.addEventListener('load', function () {
        document.body.classList.add('loaded');
    });

    // ============================================
    // Form Validation Enhancement (Optional)
    // ============================================
    const forms = document.querySelectorAll('form[data-validate]');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    // ============================================
    // Console Log (Development)
    // ============================================
    console.log('%c Medixus HMS ', 'background: linear-gradient(135deg, #4F46E5 0%, #7C3AED 100%); color: white; padding: 8px 16px; border-radius: 4px; font-weight: bold;');
    console.log('Dashboard initialized successfully ✓');

    // ============================================
    // Expose Public API (if needed)
    // ============================================
    window.HMS = {
        toggleSidebar: toggleSidebar,
        closeSidebar: closeSidebar,
        toggleTheme: toggleTheme,
        closeUserDropdown: closeUserDropdown
    };

})();