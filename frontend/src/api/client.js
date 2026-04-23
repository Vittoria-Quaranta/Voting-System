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

// Register a new voter. POST /api/register. Backend always returns 200; check body.success.
export async function registerVoter(payload) {
  const res = await fetch(`${API_BASE}/api/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  const data = await parseJson(res);
  if (!data.success) {
    throw new Error(data.message || 'Registration failed');
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

// Look up a recorded ballot by confirmation code. GET /api/vote-lookup/{code}.
// 404 means the code was not found.
export async function lookupVote(confirmationCode) {
  const code = encodeURIComponent(confirmationCode.trim());
  const res = await fetch(`${API_BASE}/api/vote-lookup/${code}`);
  if (res.status === 404) {
    throw new Error('Confirmation code not found. Check that you entered it exactly.');
  }
  if (!res.ok) {
    throw new Error('Lookup failed, please try again.');
  }
  return parseJson(res);
}

// Third-party participation check. GET /api/participation?username=...
// Returns { voted: bool }. Unknown usernames are reported as voted=false.
export async function checkParticipation(username) {
  const q = encodeURIComponent(username.trim());
  const res = await fetch(`${API_BASE}/api/participation?username=${q}`);
  if (res.status === 400) {
    throw new Error('Please enter a username.');
  }
  if (!res.ok) {
    throw new Error('Participation check failed, please try again.');
  }
  return parseJson(res);
}
