/* ===================================================
   AgroTrip India – Main JavaScript
   =================================================== */

"use strict";

/* -------- PAGE LOADER -------- */
window.addEventListener('load', () => {
    setTimeout(() => {
        const loader = document.getElementById('pageLoader');
        if (loader) loader.classList.add('hidden');
    }, 1900);
});

/* -------- NAVBAR SCROLL -------- */
const navbar = document.getElementById('mainNavbar');
window.addEventListener('scroll', () => {
    if (window.scrollY > 60) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
    toggleBackToTop();
}, { passive: true });

/* -------- SMOOTH SCROLL FOR NAV LINKS -------- */
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href === '#') return;
        const target = document.querySelector(href);
        if (target) {
            e.preventDefault();
            const offset = 80;
            const top = target.getBoundingClientRect().top + window.pageYOffset - offset;
            window.scrollTo({ top, behavior: 'smooth' });
            // Close mobile menu
            const navMenu = document.getElementById('navMenu');
            if (navMenu && navMenu.classList.contains('show')) {
                const bsCollapse = bootstrap.Collapse.getInstance(navMenu);
                if (bsCollapse) bsCollapse.hide();
            }
        }
    });
});

/* -------- BACK TO TOP -------- */
const backToTopBtn = document.getElementById('backToTop');
function toggleBackToTop() {
    if (backToTopBtn) {
        if (window.scrollY > 400) {
            backToTopBtn.classList.add('visible');
        } else {
            backToTopBtn.classList.remove('visible');
        }
    }
}

/* -------- HERO PARTICLES -------- */
function createParticles() {
    const container = document.getElementById('heroParticles');
    if (!container) return;
    const count = 18;
    for (let i = 0; i < count; i++) {
        const p = document.createElement('div');
        p.classList.add('particle');
        const size = Math.random() * 12 + 4;
        const left = Math.random() * 100;
        const dur = Math.random() * 12 + 8;
        const delay = Math.random() * 10;
        p.style.cssText = `
      width: ${size}px; height: ${size}px;
      left: ${left}%;
      animation-duration: ${dur}s;
      animation-delay: ${delay}s;
      opacity: ${Math.random() * 0.25 + 0.05};
    `;
        container.appendChild(p);
    }
}
createParticles();

/* -------- SCROLL REVEAL -------- */
function initReveal() {
    const els = document.querySelectorAll('.reveal-up, .reveal-left, .reveal-right');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });
    els.forEach(el => observer.observe(el));
}
initReveal();

/* -------- ANIMATED COUNTER -------- */
function animateCounter(el) {
    const target = parseInt(el.getAttribute('data-target'));
    const isDecimal = el.getAttribute('data-decimal') === 'true';
    const duration = 2000;
    const step = 16;
    const increment = target / (duration / step);
    let current = 0;

    const timer = setInterval(() => {
        current += increment;
        if (current >= target) {
            current = target;
            clearInterval(timer);
        }
        if (isDecimal) {
            el.textContent = (current / 10).toFixed(1);
        } else {
            el.textContent = Math.floor(current).toLocaleString('en-IN');
        }
    }, step);
}

function initCounters() {
    const counters = document.querySelectorAll('.stat-num');
    const section = document.querySelector('.stats-section');
    if (!section) return;
    const obs = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                counters.forEach(c => animateCounter(c));
                obs.unobserve(entry.target);
            }
        });
    }, { threshold: 0.3 });
    obs.observe(section);
}
initCounters();

/* -------- TOAST -------- */
function showToast(message = 'Thank you for your feedback!', type = 'success') {
    const toastEl = document.getElementById('mainToast');
    const toastMsg = document.getElementById('toastMsg');
    const toastIcon = document.getElementById('toastIcon');
    if (!toastEl) return;

    toastMsg.textContent = message;

    if (type === 'success') {
        toastIcon.className = 'fa-solid fa-circle-check text-accent fs-5';
    } else if (type === 'error') {
        toastIcon.className = 'fa-solid fa-circle-xmark text-danger fs-5';
    } else {
        toastIcon.className = 'fa-solid fa-circle-info text-info fs-5';
    }

    const toast = new bootstrap.Toast(toastEl, { delay: 4000 });
    toast.show();
}

/* -------- FEEDBACK FORM -------- */
const feedbackForm = document.getElementById('feedbackForm');
if (feedbackForm) {
    feedbackForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const name = document.getElementById('fbName').value.trim();
        const email = document.getElementById('fbEmail').value.trim();
        const rating = document.getElementById('fbRating').value;
        const comment = document.getElementById('fbComment').value.trim();

        // Simple validation
        if (!name) { shakeField('fbName'); return; }
        if (!email || !isValidEmail(email)) { shakeField('fbEmail'); return; }
        if (!rating) { shakeField('fbRating'); return; }
        if (!comment) { shakeField('fbComment'); return; }

        // Show loading state
        const btnText = document.getElementById('fbBtnText');
        const btnSpinner = document.getElementById('fbBtnSpinner');
        btnText.classList.add('d-none');
        btnSpinner.classList.remove('d-none');

        // Simulate API call
        setTimeout(() => {
            btnText.classList.remove('d-none');
            btnSpinner.classList.add('d-none');
            feedbackForm.reset();
            showToast('🌾 Thank you for your feedback! We appreciate your support.', 'success');
        }, 1800);
    });
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function shakeField(id) {
    const el = document.getElementById(id);
    if (!el) return;
    el.style.animation = 'none';
    el.offsetHeight; // reflow
    el.style.animation = 'shake 0.4s ease';
    el.focus();
    el.addEventListener('animationend', () => { el.style.animation = ''; }, { once: true });
}

// Inject shake keyframe
const shakeStyle = document.createElement('style');
shakeStyle.textContent = `
  @keyframes shake {
    0%,100% { transform: translateX(0); }
    20%,60% { transform: translateX(-6px); }
    40%,80% { transform: translateX(6px); }
  }
`;
document.head.appendChild(shakeStyle);

/* -------- SEARCH HANDLER -------- */
function handleSearch() {
    const dest = document.getElementById('searchDestination').value.trim();
    const checkin = document.getElementById('searchCheckin').value;
    const checkout = document.getElementById('searchCheckout').value;

    if (!dest) {
        showToast('Please enter a destination to search!', 'error');
        document.getElementById('searchDestination').focus();
        return;
    }
    if (!checkin || !checkout) {
        showToast('Please select check-in and check-out dates!', 'error');
        return;
    }
    if (checkin >= checkout) {
        showToast('Check-out date must be after check-in date!', 'error');
        return;
    }

    showToast(`🔍 Searching farmhouses in ${dest}...`, 'info');
    // Simulate search
    setTimeout(() => {
        showToast(`✅ Found 12 farmhouses in ${dest}! Explore below.`, 'success');
        document.getElementById('farmhouses').scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 1200);
}

/* -------- WISHLIST TOGGLE -------- */
function toggleWishlist(btn) {
    btn.classList.toggle('active');
    const icon = btn.querySelector('i');
    if (btn.classList.contains('active')) {
        icon.classList.replace('fa-regular', 'fa-solid');
        showToast('❤️ Added to your wishlist!', 'success');
    } else {
        icon.classList.replace('fa-solid', 'fa-regular');
        showToast('Removed from wishlist.', 'info');
    }
}

/* -------- DATE DEFAULTS -------- */
(function setDateDefaults() {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
    const dayAfter = new Date(today);
    dayAfter.setDate(today.getDate() + 2);

    const fmt = d => d.toISOString().split('T')[0];
    const checkinEl = document.getElementById('searchCheckin');
    const checkoutEl = document.getElementById('searchCheckout');
    if (checkinEl) { checkinEl.value = fmt(tomorrow); checkinEl.min = fmt(tomorrow); }
    if (checkoutEl) { checkoutEl.value = fmt(dayAfter); checkoutEl.min = fmt(dayAfter); }

    if (checkinEl) {
        checkinEl.addEventListener('change', function () {
            const newMin = new Date(this.value);
            newMin.setDate(newMin.getDate() + 1);
            checkoutEl.min = fmt(newMin);
            if (checkoutEl.value <= this.value) {
                checkoutEl.value = fmt(newMin);
            }
        });
    }
})();

/* -------- KEYBOARD SEARCH -------- */
const searchInput = document.getElementById('searchDestination');
if (searchInput) {
    searchInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') handleSearch();
    });
}

/* -------- CARD HOVER TILT EFFECT -------- */
function initTilt() {
    const cards = document.querySelectorAll('.farm-card, .dest-card, .exp-card');
    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const cx = rect.width / 2;
            const cy = rect.height / 2;
            const rotX = ((y - cy) / cy) * 3;
            const rotY = ((cx - x) / cx) * 3;
            card.style.transform = `perspective(800px) rotateX(${rotX}deg) rotateY(${rotY}deg) translateY(-8px)`;
        });
        card.addEventListener('mouseleave', () => {
            card.style.transform = '';
        });
    });
}
initTilt();

/* -------- ACTIVE NAV HIGHLIGHT -------- */
function initActiveNav() {
    const sections = document.querySelectorAll('section[id]');
    const navLinks = document.querySelectorAll('.nav-link[href^="#"]');

    const obs = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                navLinks.forEach(link => link.classList.remove('active'));
                const active = document.querySelector(`.nav-link[href="#${entry.target.id}"]`);
                if (active) active.classList.add('active');
            }
        });
    }, { threshold: 0.4 });

    sections.forEach(s => obs.observe(s));
}
initActiveNav();

/* -------- ACTIVITY CARD CONTROLS LAYOUT -------- */
(function layoutCarouselControls() {
    ['activitiesCarousel', 'reviewCarousel'].forEach(id => {
        const carousel = document.getElementById(id);
        if (!carousel) return;
        const prev = carousel.querySelector('.carousel-control-prev');
        const next = carousel.querySelector('.carousel-control-next');
        if (!prev || !next) return;

        const wrapper = document.createElement('div');
        wrapper.classList.add('carousel-controls-row', 'mt-4');
        prev.style.cssText = '';
        next.style.cssText = '';
        carousel.parentNode.insertBefore(wrapper, carousel.nextSibling);
        wrapper.appendChild(prev);
        wrapper.appendChild(next);
    });
})();

/* -------- BUTTON GLOW ON HOVER -------- */
(function initButtonGlow() {
    document.querySelectorAll('.btn-primary-brand, .btn-cta, .btn-search, .btn-farm-view').forEach(btn => {
        btn.addEventListener('mouseenter', function () {
            this.style.filter = 'brightness(1.08)';
        });
        btn.addEventListener('mouseleave', function () {
            this.style.filter = '';
        });
    });
})();

console.log('%c🌾 AgroTrip India – Powered by Rural Passion', 'color: #2d6a4f; font-size: 14px; font-weight: bold;');
