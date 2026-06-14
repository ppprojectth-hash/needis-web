// Scroll-reveal via IntersectionObserver.
// Adds .js-anim-ready to <html> first so CSS can safely hide .reveal elements
// knowing JS is active. Falls back gracefully if IntersectionObserver absent.
(function () {
    document.documentElement.classList.add('js-anim-ready');

    var revealEls = document.querySelectorAll('.reveal');
    if (!revealEls.length) return;

    if (!('IntersectionObserver' in window)) {
        revealEls.forEach(function (el) { el.classList.add('active'); });
        return;
    }

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('active');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    revealEls.forEach(function (el) { observer.observe(el); });
})();

// Navbar elevated shadow when user has scrolled past the top.
(function () {
    var navbar = document.querySelector('.site-navbar');
    if (!navbar) return;

    function onScroll() {
        navbar.classList.toggle('navbar-scrolled', window.scrollY > 20);
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
})();
