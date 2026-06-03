// Apply saved theme before paint to avoid flash
(function () {
    const saved = localStorage.getItem('pawdopt-theme');
    if (saved === 'dark') document.documentElement.setAttribute('data-theme', 'dark');
})();

document.addEventListener('DOMContentLoaded', function () {

    // ── Dark mode toggle ──────────────────────────────
    const darkBtn = document.querySelector('.nav-icon-btn[title="Toggle dark mode"]');
    if (darkBtn) {
        const updateIcon = (isDark) => {
            darkBtn.innerHTML = isDark
                ? `<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                     <circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/>
                     <line x1="12" y1="21" x2="12" y2="23"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/>
                     <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/><line x1="1" y1="12" x2="3" y2="12"/>
                     <line x1="21" y1="12" x2="23" y2="12"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/>
                     <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/>
                   </svg>`
                : `<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                     <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
                   </svg>`;
        };

        const isDark = () => document.documentElement.getAttribute('data-theme') === 'dark';
        updateIcon(isDark());

        darkBtn.addEventListener('click', function () {
            const next = isDark() ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            localStorage.setItem('pawdopt-theme', next);
            updateIcon(next === 'dark');
        });
    }

    // ── FAQ accordion ─────────────────────────────────
    document.querySelectorAll('.faq-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const id = btn.getAttribute('data-faq');
            const answer = document.getElementById('faq-' + id);
            const isOpen = btn.getAttribute('aria-expanded') === 'true';

            // Close all
            document.querySelectorAll('.faq-btn').forEach(b => b.setAttribute('aria-expanded', 'false'));
            document.querySelectorAll('.faq-answer').forEach(a => a.classList.remove('open'));

            // Open clicked if it was closed
            if (!isOpen) {
                btn.setAttribute('aria-expanded', 'true');
                answer.classList.add('open');
            }
        });
    });

});
