const API_BASE = import.meta.env.VITE_API_BASE || '';

export async function fetchBallot() {
  const res = await fetch(`${API_BASE}/api/ballot`);
  if (!res.ok) throw new Error('Failed to fetch ballot');
  return res.json();
}

// Authenticate with username/password. Backend endpoint: POST /api/login
// Backend always returns 200; check response body 'success' field.
export async function authenticate(username, password) {
  const res = await fetch(`${API_BASE}/api/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });

  const data = await res.json();
  if (!data.success) {
    throw new Error(data.message || 'Authentication failed');
  }
  return data;
}

// Submit ballot to POST /api/submit-ballot. Backend always returns 200; check body.success.
export async function submitBallot(payload) {
  const res = await fetch(`${API_BASE}/api/submit-ballot`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  const data = await res.json();
  if (!data.success) {
    throw new Error(data.message || 'Submit failed');
  }
  return data;
}
