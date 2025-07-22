// Enhanced dark theme site interactions and animations

document.addEventListener('DOMContentLoaded', function () {
    // Initialize all enhanced features
    initializeScrollAnimations();
    initializeParallaxEffects();
    initializeButtonAnimations();
    initializeCounterAnimations();
    initializeTypewriterEffect();
    initializeGlowEffects();
    initializeParticleBackground();
});

// Scroll-triggered animations with enhanced dark theme effects
function initializeScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -100px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');

                // Enhanced stagger animation for child elements
                const children = entry.target.querySelectorAll('.feature-card, .testimonial-card');
                children.forEach((child, index) => {
                    setTimeout(() => {
                        child.style.opacity = '1';
                        child.style.transform = 'translateY(0) scale(1)';
                        child.style.filter = 'blur(0px)';
                    }, index * 200);
                });
            }
        });
    }, observerOptions);

    // Observe sections
    document.querySelectorAll('.features-section, .testimonials-section').forEach(section => {
        observer.observe(section);
    });
}

// Enhanced parallax scrolling effects for dark theme
function initializeParallaxEffects() {
    let ticking = false;

    function updateParallax() {
        const scrolled = window.pageYOffset;

        // Hero parallax with enhanced dark theme effects
        const heroSection = document.querySelector('.hero-section');
        if (heroSection) {
            heroSection.style.transform = `translateY(${scrolled * 0.2}px)`;

            // Add dynamic glow effect based on scroll
            const glowIntensity = Math.min(scrolled / 500, 1);
            heroSection.style.filter = `drop-shadow(0 0 ${20 + glowIntensity * 30}px rgba(99, 102, 241, ${0.1 + glowIntensity * 0.2}))`;
        }

        // Floating elements parallax
        document.querySelectorAll('.feature-icon').forEach((element, index) => {
            const speed = 0.3 + (index * 0.1);
            const rotation = scrolled * 0.05;
            element.style.transform = `translateY(${scrolled * speed}px) rotate(${rotation}deg)`;
        });

        ticking = false;
    }

    function requestTick() {
        if (!ticking) {
            requestAnimationFrame(updateParallax);
            ticking = true;
        }
    }

    window.addEventListener('scroll', requestTick);
}

// Enhanced button interactions with dark theme effects
function initializeButtonAnimations() {
    document.querySelectorAll('.btn').forEach(button => {
        // Enhanced ripple effect with glow
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');

            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 800);
        });

        // Enhanced magnetic effect for large buttons
        if (button.classList.contains('btn-lg')) {
            button.addEventListener('mousemove', function (e) {
                const rect = this.getBoundingClientRect();
                const x = e.clientX - rect.left - rect.width / 2;
                const y = e.clientY - rect.top - rect.height / 2;

                this.style.transform = `translate(${x * 0.15}px, ${y * 0.15}px) translateY(-3px)`;
                this.style.filter = 'drop-shadow(0 10px 30px rgba(99, 102, 241, 0.4))';
            });

            button.addEventListener('mouseleave', function () {
                this.style.transform = '';
                this.style.filter = '';
            });
        }

        // Pulse effect on hover
        button.addEventListener('mouseenter', function () {
            this.style.animation = 'pulse 1.5s infinite';
        });

        button.addEventListener('mouseleave', function () {
            this.style.animation = '';
        });
    });

    // Enhanced ripple CSS with dark theme
    const style = document.createElement('style');
    style.textContent = `
        .btn {
            position: relative;
            overflow: hidden;
        }
        .ripple {
            position: absolute;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(255, 255, 255, 0.3) 0%, rgba(99, 102, 241, 0.2) 100%);
            transform: scale(0);
            animation: ripple-animation 0.8s cubic-bezier(0.4, 0, 0.2, 1);
            pointer-events: none;
        }
        @keyframes ripple-animation {
            to {
                transform: scale(4);
                opacity: 0;
            }
        }
        @keyframes pulse {
            0% { 
                box-shadow: 0 0 0 0 rgba(99, 102, 241, 0.7);
                filter: drop-shadow(0 0 10px rgba(99, 102, 241, 0.3));
            }
            70% { 
                box-shadow: 0 0 0 15px rgba(99, 102, 241, 0);
                filter: drop-shadow(0 0 20px rgba(99, 102, 241, 0.5));
            }
            100% { 
                box-shadow: 0 0 0 0 rgba(99, 102, 241, 0);
                filter: drop-shadow(0 0 10px rgba(99, 102, 241, 0.3));
            }
        }
    `;
    document.head.appendChild(style);
}

// Enhanced animated counters for stats with glow effects
function initializeCounterAnimations() {
    const counters = document.querySelectorAll('.stat-number');
    const observerOptions = {
        threshold: 0.5
    };

    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                counterObserver.unobserve(entry.target);
            }
        });
    }, observerOptions);

    counters.forEach(counter => {
        counterObserver.observe(counter);
    });
}

function animateCounter(element) {
    const text = element.textContent;
    const number = parseInt(text.replace(/[^0-9]/g, ''));
    const suffix = text.replace(/[0-9]/g, '');
    const duration = 2500;
    const increment = number / (duration / 16);
    let current = 0;

    const timer = setInterval(() => {
        current += increment;
        if (current >= number) {
            current = number;
            clearInterval(timer);
            // Add final glow effect
            element.style.filter = 'drop-shadow(0 0 15px rgba(99, 102, 241, 0.5))';
        }

        let displayNumber = Math.floor(current);
        if (displayNumber >= 1000000) {
            displayNumber = (displayNumber / 1000000).toFixed(1) + 'M';
        } else if (displayNumber >= 1000) {
            displayNumber = (displayNumber / 1000).toFixed(1) + 'K';
        }

        element.textContent = displayNumber + suffix.replace(/[0-9.KM]/g, '');

        // Add progressive glow during animation
        const progress = current / number;
        element.style.filter = `drop-shadow(0 0 ${5 + progress * 10}px rgba(99, 102, 241, ${0.2 + progress * 0.3}))`;
    }, 16);
}

// Enhanced typewriter effect for hero title
function initializeTypewriterEffect() {
    const heroTitle = document.querySelector('.hero-title');
    if (!heroTitle) return;

    const text = heroTitle.textContent;
    heroTitle.textContent = '';
    heroTitle.style.opacity = '1';

    let index = 0;
    const typeSpeed = 40;

    function typeWriter() {
        if (index < text.length) {
            heroTitle.textContent += text.charAt(index);

            // Add glow effect while typing
            heroTitle.style.filter = `drop-shadow(0 0 ${10 + Math.random() * 5}px rgba(99, 102, 241, 0.3))`;

            index++;
            setTimeout(typeWriter, typeSpeed);
        } else {
            // Add blinking cursor with glow
            const cursor = document.createElement('span');
            cursor.textContent = '|';
            cursor.style.animation = 'blink 1s infinite';
            cursor.style.filter = 'drop-shadow(0 0 5px rgba(99, 102, 241, 0.8))';
            heroTitle.appendChild(cursor);

            // Remove cursor after 3 seconds
            setTimeout(() => {
                cursor.remove();
            }, 3000);
        }
    }

    // Start typewriter effect after a delay
    setTimeout(typeWriter, 1200);

    // Enhanced blinking animation
    const blinkStyle = document.createElement('style');
    blinkStyle.textContent = `
        @keyframes blink {
            0%, 50% { 
                opacity: 1; 
                filter: drop-shadow(0 0 5px rgba(99, 102, 241, 0.8));
            }
            51%, 100% { 
                opacity: 0; 
                filter: drop-shadow(0 0 0px rgba(99, 102, 241, 0));
            }
        }
    `;
    document.head.appendChild(blinkStyle);
}

// Initialize glow effects for interactive elements
function initializeGlowEffects() {
    // Add glow to cards on hover
    document.querySelectorAll('.feature-card, .testimonial-card').forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.filter = 'drop-shadow(0 0 25px rgba(99, 102, 241, 0.4))';
            this.style.borderColor = 'rgba(99, 102, 241, 0.3)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.filter = '';
            this.style.borderColor = '';
        });
    });

    // Add glow to social icons
    document.querySelectorAll('.fab').forEach(icon => {
        icon.addEventListener('mouseenter', function () {
            this.style.filter = 'drop-shadow(0 0 10px rgba(99, 102, 241, 0.6))';
        });

        icon.addEventListener('mouseleave', function () {
            this.style.filter = '';
        });
    });
}

// Initialize particle background effect
function initializeParticleBackground() {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');

    canvas.style.position = 'fixed';
    canvas.style.top = '0';
    canvas.style.left = '0';
    canvas.style.width = '100%';
    canvas.style.height = '100%';
    canvas.style.pointerEvents = 'none';
    canvas.style.zIndex = '-1';
    canvas.style.opacity = '0.3';

    document.body.appendChild(canvas);

    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }

    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    const particles = [];
    const particleCount = 50;

    for (let i = 0; i < particleCount; i++) {
        particles.push({
            x: Math.random() * canvas.width,
            y: Math.random() * canvas.height,
            vx: (Math.random() - 0.5) * 0.5,
            vy: (Math.random() - 0.5) * 0.5,
            size: Math.random() * 2 + 1,
            opacity: Math.random() * 0.5 + 0.2
        });
    }

    function animateParticles() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        particles.forEach(particle => {
            particle.x += particle.vx;
            particle.y += particle.vy;

            if (particle.x < 0 || particle.x > canvas.width) particle.vx *= -1;
            if (particle.y < 0 || particle.y > canvas.height) particle.vy *= -1;

            ctx.beginPath();
            ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(99, 102, 241, ${particle.opacity})`;
            ctx.fill();

            // Add glow effect
            ctx.shadowBlur = 10;
            ctx.shadowColor = 'rgba(99, 102, 241, 0.5)';
        });

        requestAnimationFrame(animateParticles);
    }

    animateParticles();
}

// Enhanced smooth reveal animations for cards
function revealCards() {
    const cards = document.querySelectorAll('.feature-card, .testimonial-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(60px) scale(0.8)';
        card.style.filter = 'blur(5px)';
        card.style.transition = 'all 0.8s cubic-bezier(0.4, 0, 0.2, 1)';

        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0) scale(1)';
            card.style.filter = 'blur(0px)';
        }, index * 150);
    });
}

// Initialize card reveals when they come into view
const cardObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            revealCards();
            cardObserver.disconnect();
        }
    });
}, { threshold: 0.1 });

const featuresSection = document.querySelector('.features-section');
if (featuresSection) {
    cardObserver.observe(featuresSection);
}

// Enhanced navbar scroll behavior with dark theme
let lastScrollTop = 0;
const navbar = document.querySelector('.navbar-custom');

window.addEventListener('scroll', () => {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (scrollTop > lastScrollTop && scrollTop > 100) {
        // Scrolling down
        navbar.style.transform = 'translateY(-100%)';
        navbar.style.filter = 'drop-shadow(0 5px 15px rgba(0, 0, 0, 0.3))';
    } else {
        // Scrolling up
        navbar.style.transform = 'translateY(0)';
        navbar.style.filter = 'drop-shadow(0 2px 10px rgba(0, 0, 0, 0.2))';
    }

    lastScrollTop = scrollTop;
}, false);

// Add mouse trail effect
document.addEventListener('mousemove', (e) => {
    const trail = document.createElement('div');
    trail.style.position = 'fixed';
    trail.style.left = e.clientX + 'px';
    trail.style.top = e.clientY + 'px';
    trail.style.width = '4px';
    trail.style.height = '4px';
    trail.style.background = 'radial-gradient(circle, rgba(99, 102, 241, 0.6) 0%, transparent 70%)';
    trail.style.borderRadius = '50%';
    trail.style.pointerEvents = 'none';
    trail.style.zIndex = '9999';
    trail.style.animation = 'fadeOut 0.5s ease-out forwards';

    document.body.appendChild(trail);

    setTimeout(() => {
        trail.remove();
    }, 500);
});

// Add fade out animation for mouse trail
const trailStyle = document.createElement('style');
trailStyle.textContent = `
    @keyframes fadeOut {
        0% {
            opacity: 1;
            transform: scale(1);
        }
        100% {
            opacity: 0;
            transform: scale(0);
        }
    }
`;
document.head.appendChild(trailStyle);

// Enhanced loading screen with dark theme
window.addEventListener('load', () => {
    const loadingOverlay = document.getElementById('loadingOverlay');
    if (loadingOverlay) {
        setTimeout(() => {
            loadingOverlay.style.opacity = '0';
            loadingOverlay.style.filter = 'blur(10px)';
            setTimeout(() => {
                loadingOverlay.style.display = 'none';
            }, 500);
        }, 800);
    }
});