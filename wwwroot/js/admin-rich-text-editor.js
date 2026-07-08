// Toolbar for textarea.rich-text-editor fields — lets non-technical admins format
// text (bold, bullet list, numbered list, heading, preview) without typing Markdown
// syntax by hand. The textarea still stores plain Markdown; MarkdownService remains
// the single source of truth for what gets rendered publicly (see the Preview action,
// which calls the server so the preview matches real rendering).
(function () {
    'use strict';

    function insertAtCursor(textarea, text) {
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        textarea.value = textarea.value.slice(0, start) + text + textarea.value.slice(end);
        var newPos = start + text.length;
        textarea.focus();
        textarea.setSelectionRange(newPos, newPos);
        notifyChange(textarea);
    }

    function wrapSelection(textarea, before, after, placeholder) {
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var selected = textarea.value.slice(start, end);
        var text = selected || placeholder;
        textarea.value = textarea.value.slice(0, start) + before + text + after + textarea.value.slice(end);

        var selStart = start + before.length;
        var selEnd = selStart + text.length;
        textarea.focus();
        textarea.setSelectionRange(selStart, selEnd);
        notifyChange(textarea);
    }

    function getSelectedLineRange(textarea) {
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var value = textarea.value;
        var lineStart = value.lastIndexOf('\n', Math.max(start - 1, 0)) + 1;
        var lineEndIdx = value.indexOf('\n', end);
        var lineEnd = lineEndIdx === -1 ? value.length : lineEndIdx;
        return { lineStart: lineStart, lineEnd: lineEnd };
    }

    function applyListFormat(textarea, kind, isTh) {
        var hasSelection = textarea.selectionStart !== textarea.selectionEnd;

        if (!hasSelection) {
            var sample = kind === 'bullet'
                ? (isTh ? '- รายการที่ 1\n- รายการที่ 2' : '- Item one\n- Item two')
                : (isTh ? '1. รายการที่ 1\n2. รายการที่ 2' : '1. Item one\n2. Item two');
            insertAtCursor(textarea, sample);
            return;
        }

        var range = getSelectedLineRange(textarea);
        var block = textarea.value.slice(range.lineStart, range.lineEnd);
        var lines = block.split('\n');
        var counter = 1;
        var newLines = lines.map(function (line) {
            if (line.trim() === '') return line;
            var stripped = line.replace(/^\s*(?:[-*]|\d+\.)\s+/, '');
            return kind === 'bullet' ? '- ' + stripped : (counter++) + '. ' + stripped;
        });
        var newBlock = newLines.join('\n');

        textarea.value = textarea.value.slice(0, range.lineStart) + newBlock + textarea.value.slice(range.lineEnd);
        textarea.focus();
        textarea.setSelectionRange(range.lineStart, range.lineStart + newBlock.length);
        notifyChange(textarea);
    }

    function applyHeading(textarea, isTh) {
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var selected = textarea.value.slice(start, end);

        if (selected) {
            var text = '## ' + selected;
            textarea.value = textarea.value.slice(0, start) + text + textarea.value.slice(end);
            textarea.focus();
            textarea.setSelectionRange(start, start + text.length);
        } else {
            var placeholder = isTh ? 'หัวข้อ' : 'Heading';
            var needsNewline = start > 0 && textarea.value.charAt(start - 1) !== '\n';
            var prefix = (needsNewline ? '\n' : '') + '## ';
            textarea.value = textarea.value.slice(0, start) + prefix + placeholder + textarea.value.slice(end);
            var selStart = start + prefix.length;
            textarea.focus();
            textarea.setSelectionRange(selStart, selStart + placeholder.length);
        }
        notifyChange(textarea);
    }

    function applyBold(textarea, isTh) {
        wrapSelection(textarea, '**', '**', isTh ? 'ตัวหนา' : 'bold text');
    }

    function notifyChange(textarea) {
        textarea.dispatchEvent(new Event('input', { bubbles: true }));
    }

    function getAntiForgeryToken(form) {
        var scope = form || document;
        var input = scope.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : null;
    }

    function togglePreview(textarea) {
        var previewWrap = document.getElementById(textarea.id + '-preview');
        if (!previewWrap) return;

        if (previewWrap.classList.contains('is-visible')) {
            previewWrap.classList.remove('is-visible');
            return;
        }

        var contentEl = previewWrap.querySelector('.rich-content');
        contentEl.innerHTML = '';
        previewWrap.classList.add('is-visible');

        var form = textarea.closest('form');
        var token = getAntiForgeryToken(form);
        var body = new FormData();
        body.append('markdown', textarea.value);
        if (token) body.append('__RequestVerificationToken', token);

        fetch('/Admin/MarkdownPreview/Preview', {
            method: 'POST',
            body: body,
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
        })
            .then(function (res) {
                if (!res.ok) throw new Error('Preview request failed');
                return res.json();
            })
            .then(function (data) {
                contentEl.innerHTML = data && data.safeHtml
                    ? data.safeHtml
                    : '<span class="text-muted small">Nothing to preview.</span>';
            })
            .catch(function () {
                contentEl.innerHTML = '<span class="text-danger small">Preview unavailable.</span>';
            });
    }

    document.addEventListener('click', function (e) {
        var btn = e.target.closest('[data-action]');
        if (!btn) return;

        var toolbar = btn.closest('.rich-text-toolbar');
        if (!toolbar) return;

        var targetSelector = toolbar.getAttribute('data-target');
        var textarea = targetSelector ? document.querySelector(targetSelector) : null;
        if (!textarea) return;

        e.preventDefault();

        var isTh = textarea.getAttribute('data-lang') === 'th';
        var action = btn.getAttribute('data-action');

        switch (action) {
            case 'bold':
                applyBold(textarea, isTh);
                break;
            case 'bullet':
                applyListFormat(textarea, 'bullet', isTh);
                break;
            case 'number':
                applyListFormat(textarea, 'number', isTh);
                break;
            case 'heading':
                applyHeading(textarea, isTh);
                break;
            case 'preview':
                togglePreview(textarea);
                break;
        }
    });
})();
