/* ============================================================
   site.js — InsureTrust Web Client
   ============================================================ */

'use strict';

(function () {

    // ── Helpers ──────────────────────────────────────────────────────────────

    function getAuthToken() {
        return document.cookie
            .split(';')
            .map(c => c.trim())
            .find(c => c.startsWith('authToken='))
            ?.split('=')[1] ?? null;
    }

    // ── Calculator Popup ─────────────────────────────────────────────────────

    const openBtn  = document.getElementById('openCalculatorPanel');
    const closeBtn = document.getElementById('closeCalculatorPopup');
    const popup    = document.getElementById('calculatorPopup');

    function openCalc()  { if (popup) { popup.classList.add('open');    popup.setAttribute('aria-hidden', 'false'); } }
    function closeCalc() { if (popup) { popup.classList.remove('open'); popup.setAttribute('aria-hidden', 'true');  } }

    if (openBtn)  openBtn.addEventListener('click', openCalc);
    if (closeBtn) closeBtn.addEventListener('click', closeCalc);

    // Close on backdrop click
    if (popup) {
        popup.addEventListener('click', function (e) {
            if (e.target === popup) closeCalc();
        });
    }

    // Close on Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeCalc();
    });

    // ── Calculator — Run Estimate ────────────────────────────────────────────
    // Wires up ALL .js-calculator panels (popup + dedicated Calculator page).

    document.querySelectorAll('.js-calculator').forEach(function (panel) {

        const runBtn     = panel.querySelector('.js-calc-run');
        const resultEl   = panel.querySelector('.js-calc-result');
        const ageEl      = panel.querySelector('.js-calc-age');
        const amountEl   = panel.querySelector('.js-calc-amount');
        const tenureEl   = panel.querySelector('.js-calc-tenure');
        const categoryEl = panel.querySelector('.js-calc-category');

        if (!runBtn) return;

        runBtn.addEventListener('click', async function () {
            if (resultEl) resultEl.textContent = 'Calculating…';

            const payload = {
                age:            parseInt(ageEl?.value   || '0', 10),
                packageAmount:  parseFloat(amountEl?.value || '0'),
                tenure:         parseInt(tenureEl?.value  || '0', 10),
                policyCategory: categoryEl?.value || null
            };

            // Basic client-side validation
            if (!payload.age || !payload.packageAmount || !payload.tenure) {
                if (resultEl) resultEl.textContent = '\u26a0 Please fill in Age, Package Amount, and Tenure.';
                return;
            }

            try {
                // POST to the MVC CalculatorController, which calls the gateway -> CalculatorService
                const resp = await fetch('/Calculator/Estimate', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (!resp.ok) {
                    if (resultEl) resultEl.textContent = '\u26a0 Server error (' + resp.status + '). Is the calculator service running?';
                    return;
                }

                const json = await resp.json();

                if (json.success && json.data) {
                    const d = json.data;
                    if (resultEl) {
                        resultEl.innerHTML =
                            '<strong>Estimated Premium:</strong> \u20b9' + d.estimatedPremium.toLocaleString('en-IN') +
                            ' &nbsp;|&nbsp; <strong>Total Investment:</strong> \u20b9' + d.totalInvestment.toLocaleString('en-IN') +
                            ' &nbsp;|&nbsp; <strong>Maturity Amount:</strong> \u20b9' + d.maturityAmount.toLocaleString('en-IN') +
                            (d.breakup ? '<br><small>' + d.breakup + '</small>' : '');
                    }
                } else {
                    if (resultEl) resultEl.textContent = '\u26a0 ' + (json.message || 'Could not calculate estimate.');
                }

            } catch (err) {
                if (resultEl) resultEl.textContent = '\u26a0 Network error. Please try again.';
                console.error('Calculator error:', err);
            }
        });
    });

    // ── Notification Bell — Unread Count ────────────────────────────────────

    async function refreshUnreadCount() {
        const token = getAuthToken();
        if (!token) return;

        const badge = document.getElementById('notifBadge');
        if (!badge) return;

        try {
            const resp = await fetch('/Notification/UnreadCount');
            if (resp.ok) {
                const json = await resp.json();
                const count = json?.count ?? json?.data?.count ?? 0;
                badge.textContent = count > 0 ? count : '';
                badge.style.display = count > 0 ? 'inline-block' : 'none';
            }
        } catch { /* silently ignore */ }
    }

    refreshUnreadCount();
    setInterval(refreshUnreadCount, 60000); // poll every 60 s

    // ── Sidebar User Profile ─────────────────────────────────────────────────

    async function loadSidebarProfile() {
        const token = getAuthToken();
        if (!token) return;

        try {
            const resp = await fetch('/Account/ProfileData');
            if (!resp.ok) return;
            const json = await resp.json();
            if (!json.success || !json.data) return;

            const data   = json.data;
            const nameEl  = document.getElementById('sidebarUserName');
            const emailEl = document.getElementById('sidebarUserEmail');

            if (nameEl  && data.name)  nameEl.textContent  = data.name;
            if (emailEl && data.email) emailEl.textContent = data.email;
        } catch { /* silently ignore */ }
    }

    loadSidebarProfile();

    // ── Auth-Required Navigation Guard ──────────────────────────────────────
    // Links/buttons marked data-auth-required="true" redirect to login if
    // the user has no auth token cookie.

    document.querySelectorAll('[data-auth-required="true"]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            if (!getAuthToken()) {
                e.preventDefault();
                const href = el.getAttribute('href') || window.location.pathname;
                window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(href);
            }
        });
    });

})();
