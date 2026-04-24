import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { authenticate } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Alert } from '../components/ui/Alert';

const API_BASE = import.meta.env.VITE_API_BASE || '';

const TEST_ACCOUNTS = [
  { username: 'tfrazier', label: 'Tommie Frazier' },
  { username: 'ecrouch', label: 'Eric Crouch' },
  { username: 'bberringer', label: 'Brook Berringer' },
  { username: 'jrodgers', label: 'Johnny Rodgers' },
  { username: 'mrozier', label: 'Mike Rozier' },
];

export default function DevTools() {
  const navigate = useNavigate();
  const { setCurrentVoter, markVoted, logout } = useVoting();
  const [status, setStatus] = useState(null);
  const [resetting, setResetting] = useState(false);
  const [loggingIn, setLoggingIn] = useState(null);

  async function handleResetVotes() {
    setResetting(true);
    try {
      const res = await fetch(`${API_BASE}/api/dev/reset-votes`, { method: 'POST' });
      const data = await res.json();
      setStatus({ type: 'success', message: data.message });
      logout();
    } catch {
      setStatus({ type: 'error', message: 'Failed to reset votes. Is the backend running in Development mode?' });
    }
    setResetting(false);
  }

  async function handleQuickLogin(account) {
    setLoggingIn(account.username);
    logout();
    try {
      const data = await authenticate(account.username, 'husker2026');
      setCurrentVoter({
        voterId: data.voterId,
        firstName: data.firstName,
        lastName: data.lastName,
      });
      if (data.hasVoted) {
        markVoted(null);
      }
      setStatus({ type: 'success', message: `Logged in as ${data.firstName} ${data.lastName}${data.hasVoted ? ' (already voted)' : ''}` });
    } catch (err) {
      setStatus({ type: 'error', message: err.message });
    }
    setLoggingIn(null);
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="mb-8">
        <h2 className="text-2xl font-bold mb-1">Dev / Demo Tools</h2>
        <p className="text-[var(--color-muted)]">
          Development utilities for testing and demos. Not available in production.
        </p>
      </div>

      {status && (
        <Alert variant={status.type === 'success' ? 'success' : 'error'} className="mb-6">
          {status.message}
        </Alert>
      )}

      <div className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle>Reset Votes</CardTitle>
            <CardDescription>
              Clear all votes and voter records from the database. Use before demos to start fresh.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="danger" onClick={handleResetVotes} disabled={resetting}>
              {resetting ? 'Resetting...' : 'Reset All Votes'}
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Quick Login</CardTitle>
            <CardDescription>
              One-click login with test accounts. All use password {'"husker2026"'}.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-wrap gap-2">
              {TEST_ACCOUNTS.map((account) => (
                <Button
                  key={account.username}
                  variant="outline"
                  size="sm"
                  onClick={() => handleQuickLogin(account)}
                  disabled={loggingIn !== null}
                >
                  {loggingIn === account.username ? 'Logging in...' : account.label}
                </Button>
              ))}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Quick Navigation</CardTitle>
            <CardDescription>Jump to any page directly.</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-wrap gap-2">
              {['/', '/ballot', '/review', '/confirmation', '/results', '/lookup', '/participation'].map((path) => (
                <Button
                  key={path}
                  variant="ghost"
                  size="sm"
                  onClick={() => navigate(path)}
                >
                  {path}
                </Button>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
