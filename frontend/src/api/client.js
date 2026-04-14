const API_BASE = import.meta.env.VITE_API_BASE || ''; // set VITE_API_BASE in .env when needed

export async function fetchBallot() {
  const res = await fetch(`${API_BASE}/api/ballot`);
  if (!res.ok) throw new Error('Failed to fetch ballot');
  return res.json();
}

export async function authenticate(voterId, pin) {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ voterId, pin }),
  });
  if (!res.ok) throw new Error('Authentication failed');
  return res.json();
}

export async function submitBallot(payload) {
  const res = await fetch(`${API_BASE}/api/ballot/submit`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!res.ok) throw new Error('Submit failed');
  return res.json();
}
