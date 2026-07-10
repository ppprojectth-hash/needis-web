// Reusable Admin upload preview: wires <input class="js-image-preview-input"> (and
// PDF/file-only inputs marked "js-file-meta-input") to their preview <img>, filename,
// and size targets via data-* attributes. Runs once after DOMContentLoaded and supports
// any number of independent upload fields on the same page.
document.addEventListener('DOMContentLoaded', function () {
    var inputs = document.querySelectorAll('.js-image-preview-input, .js-file-meta-input');

    inputs.forEach(function (input) {
        input.addEventListener('change', function () {
            var file = input.files && input.files[0];

            var previewSelector  = input.dataset.previewTarget;
            var wrapperSelector  = input.dataset.previewWrapper;
            var filenameSelector = input.dataset.filenameTarget;
            var sizeSelector     = input.dataset.sizeTarget;
            var warningSelector  = input.dataset.warningTarget;

            var preview   = previewSelector  ? document.querySelector(previewSelector)  : null;
            var wrapper   = wrapperSelector  ? document.querySelector(wrapperSelector)  : null;
            var filenameEl = filenameSelector ? document.querySelector(filenameSelector) : null;
            var sizeEl    = sizeSelector     ? document.querySelector(sizeSelector)     : null;
            var warningEl = warningSelector  ? document.querySelector(warningSelector)  : null;

            if (warningEl) {
                warningEl.textContent = '';
                warningEl.classList.add('d-none');
            }

            if (!file) return;

            if (filenameEl) filenameEl.textContent = file.name;
            if (sizeEl) sizeEl.textContent = formatFileSize(file.size);

            // Non-image files (e.g. PDF): show filename/size only, never crash, never
            // attempt an image preview.
            if (!file.type || !file.type.startsWith('image/')) {
                if (input.classList.contains('js-image-preview-input') && warningEl) {
                    warningEl.textContent = 'Selected file is not an image — no preview available.';
                    warningEl.classList.remove('d-none');
                }
                return;
            }

            if (!preview) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                preview.src = e.target.result;
                preview.classList.remove('d-none', 'hidden');
                if (wrapper) wrapper.classList.remove('d-none', 'hidden');
            };
            reader.onerror = function () {
                if (warningEl) {
                    warningEl.textContent = 'Could not read the selected file.';
                    warningEl.classList.remove('d-none');
                }
            };
            reader.readAsDataURL(file);
        });
    });

    function formatFileSize(bytes) {
        if (!bytes && bytes !== 0) return '';
        if (bytes < 1024) return bytes + ' B';
        var kb = bytes / 1024;
        if (kb < 1024) return kb.toFixed(1) + ' KB';
        return (kb / 1024).toFixed(1) + ' MB';
    }
});
