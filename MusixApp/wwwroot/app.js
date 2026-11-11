// ── Window control helpers ───────────────────────────────────────────────
function sendWindowAction(action) {
  window.chrome.webview.postMessage(JSON.stringify({ action }));
}

document.getElementById('btn-close').addEventListener('click', () => sendWindowAction('close'));
document.getElementById('btn-minimize').addEventListener('click', () => sendWindowAction('minimize'));
document.getElementById('btn-maximize').addEventListener('click', () => sendWindowAction('maximize'));

// ── Backend API ping ─────────────────────────────────────────────────────
document.getElementById('pingBtn').addEventListener('click', async () => {
  const pre = document.getElementById('result');
  pre.textContent = 'Fetching…';
  try {
    const res  = await fetch('/api/ping');
    const data = await res.json();
    pre.textContent = JSON.stringify(data, null, 2);
  } catch (err) {
    pre.textContent = 'Error: ' + err.message;
  }
});
