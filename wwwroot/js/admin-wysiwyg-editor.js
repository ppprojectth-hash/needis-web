// WYSIWYG editor for .wysiwyg-editor-wrap fields — a contenteditable <div> that lets
// admins format text immediately (bold, bullet, numbered list, heading) without typing
// Markdown or clicking a Preview button. The hidden <textarea> is what actually gets
// posted to the server; the editor's HTML is synced into it on every input and again
// right before the form submits, so the existing controller/model binding is untouched.
(function () {
    'use strict';

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Lightweight one-way conversion so content saved before the WYSIWYG editor
    // existed (plain text, or the old **bold** / "- bullet" Markdown convention)
    // still looks reasonable the first time it's opened in the new editor.
    function legacyTextToHtml(text) {
        var blocks = text.replace(/\r\n/g, '\n').split(/\n{2,}/);

        var html = blocks.map(function (block) {
            var lines = block.split('\n').filter(function (l) { return l.length > 0; });
            if (lines.length === 0) return '';

            var isBulletBlock = lines.every(function (l) { return /^\s*[-*]\s+/.test(l); });
            var isNumberBlock = !isBulletBlock && lines.every(function (l) { return /^\s*\d+\.\s+/.test(l); });
            var headingMatch = lines.length === 1 && lines[0].match(/^\s*#{2,4}\s+(.*)$/);

            function inline(l) {
                return escapeHtml(l).replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
            }

            if (headingMatch) {
                return '<h3>' + inline(headingMatch[1]) + '</h3>';
            }
            if (isBulletBlock) {
                var items = lines.map(function (l) { return '<li>' + inline(l.replace(/^\s*[-*]\s+/, '')) + '</li>'; });
                return '<ul>' + items.join('') + '</ul>';
            }
            if (isNumberBlock) {
                var oitems = lines.map(function (l) { return '<li>' + inline(l.replace(/^\s*\d+\.\s+/, '')) + '</li>'; });
                return '<ol>' + oitems.join('') + '</ol>';
            }
            return '<p>' + lines.map(inline).join('<br>') + '</p>';
        }).filter(function (b) { return b.length > 0; }).join('');

        return html;
    }

    function looksLikeHtml(value) {
        return /<(p|br|strong|b|em|i|ul|ol|li|h[234]|a|div|span)\b[^>]*>/i.test(value);
    }

    function syncEditorToTextarea(editor) {
        var targetSelector = editor.getAttribute('data-target');
        var textarea = targetSelector ? document.querySelector(targetSelector) : null;
        if (!textarea) return;
        textarea.value = editor.innerHTML === '<br>' ? '' : editor.innerHTML;
    }

    function initEditor(editor) {
        var targetSelector = editor.getAttribute('data-target');
        var textarea = targetSelector ? document.querySelector(targetSelector) : null;
        if (!textarea) return;

        var initial = textarea.value || '';
        editor.innerHTML = initial === ''
            ? ''
            : (looksLikeHtml(initial) ? initial : legacyTextToHtml(initial));

        editor.addEventListener('input', function () {
            syncEditorToTextarea(editor);
        });

        // Keep the hidden textarea in sync even if something else (e.g. an in-page
        // reset) changes the editor without firing 'input'.
        editor.addEventListener('blur', function () {
            syncEditorToTextarea(editor);
        });

        var form = editor.closest('form');
        if (form && !form.__wysiwygSubmitHooked) {
            form.__wysiwygSubmitHooked = true;
            form.addEventListener('submit', function () {
                form.querySelectorAll('.wysiwyg-editor').forEach(syncEditorToTextarea);
            });
        }
    }

    function applyCommand(editor, command) {
        editor.focus();
        if (command === 'heading') {
            document.execCommand('formatBlock', false, '<h3>');
            return;
        }
        document.execCommand(command, false, null);
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.wysiwyg-editor-wrap .wysiwyg-editor').forEach(initEditor);
    });

    // mousedown (not click) + preventDefault keeps the browser from moving focus/
    // collapsing the text selection out of the editor before execCommand runs.
    document.addEventListener('mousedown', function (e) {
        var btn = e.target.closest('.wysiwyg-toolbar button[data-command]');
        if (!btn) return;
        e.preventDefault();
    });

    document.addEventListener('click', function (e) {
        var btn = e.target.closest('.wysiwyg-toolbar button[data-command]');
        if (!btn) return;

        var wrap = btn.closest('.wysiwyg-editor-wrap');
        var editor = wrap ? wrap.querySelector('.wysiwyg-editor') : null;
        if (!editor) return;

        e.preventDefault();
        applyCommand(editor, btn.getAttribute('data-command'));
        syncEditorToTextarea(editor);
    });
})();
