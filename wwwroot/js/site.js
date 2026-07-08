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

// Image upload preview: wire <input type="file" data-preview-target="#someImg"> to an <img> element.
(function () {
    document.querySelectorAll('input[type="file"][data-preview-target]').forEach(function (input) {
        var targetSelector = input.getAttribute('data-preview-target');
        var target = document.querySelector(targetSelector);
        if (!target) return;
        input.addEventListener('change', function () {
            var file = this.files && this.files[0];
            if (!file) return;
            target.src = URL.createObjectURL(file);
        });
    });
})();

// Selected-file size display: wire <input type="file" data-file-size-target="#someSpan"> to a <span>.
(function () {
    function formatBytes(bytes) {
        if (bytes < 1024)           return bytes + ' B';
        if (bytes < 1024 * 1024)   return (bytes / 1024).toFixed(1) + ' KB';
        return (bytes / 1024 / 1024).toFixed(1) + ' MB';
    }
    document.querySelectorAll('input[type="file"][data-file-size-target]').forEach(function (input) {
        var target = document.querySelector(input.getAttribute('data-file-size-target'));
        if (!target) return;
        input.addEventListener('change', function () {
            var file = this.files && this.files[0];
            target.textContent = file ? ('Selected: ' + formatBytes(file.size)) : '';
        });
    });
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
