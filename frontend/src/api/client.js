const API_BASE = import.meta.env.VITE_API_BASE || '';

// safely parse JSON, throws a readable error if the response isn't JSON
// (happens when Azure SQL is waking up and the backend returns an error page)
async function parseJson(res) {
  const text = await res.text();
  try {
    return JSON.parse(text);
  } catch {
    throw new Error('Server is starting up, please try again in a moment.');
  }
}

export async function fetchBallot() {
  const res = await fetch(`${API_BASE}/api/ballot`);
  if (!res.ok) throw new Error('Failed to fetch ballot');
  return parseJson(res);
}

// Authenticate with username/password. Backend endpoint: POST /api/login
// Backend always returns 200; check response body 'success' field.
export async function authenticate(username, password) {
  const res = await fetch(`${API_BASE}/api/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });

  const data = await parseJson(res);
  if (!data.success) {
    throw new Error(data.message || 'Authentication failed');
  }
  return data;
}

// Fetch election results. GET /api/results, 404 if no election.
export async function fetchResults() {
  const res = await fetch(`${API_BASE}/api/results`);
  if (!res.ok) throw new Error('Results not available');
  return parseJson(res);
}

// Submit ballot to POST /api/submit-ballot. Backend always returns 200; check body.success.
export async function submitBallot(payload) {
  const res = await fetch(`${API_BASE}/api/submit-ballot`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  const data = await parseJson(res);
  if (!data.success) {
    throw new Error(data.message || 'Submit failed');
  }
  return data;
}
