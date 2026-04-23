import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { authenticate } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input, Label } from '../components/ui/Input';
import { Alert } from '../components/ui/Alert';

// inline SVGs instead of pulling in a whole icon library for two glyphs
function SearchIcon() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
      className="h-5 w-5">
      <circle cx="11" cy="11" r="7" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
    </svg>
  );
}

function UsersIcon() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
      className="h-5 w-5">
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </svg>
  );
}

export default function Login() {
  const navigate = useNavigate();
  const { currentVoter, setCurrentVoter, hasVoted, markVoted } = useVoting();

  // if already logged in, skip to ballot or confirmation
  useEffect(() => {
    if (currentVoter) {
      navigate(hasVoted ? '/confirmation' : '/ballot');
    }
  }, [currentVoter, hasVoted, navigate]);

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const data = await authenticate(username, password);
      setCurrentVoter({
        voterId: data.voterId,
        firstName: data.firstName,
        lastName: data.lastName,
      });
      if (data.hasVoted) {
        markVoted(null);
        navigate('/confirmation');
      } else {
        navigate('/ballot');
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-md mx-auto">
      <div className="text-center mb-8">
        <h2 className="text-2xl font-bold mb-2">Voter Login</h2>
        <p className="text-[var(--color-muted)]">
          Enter your credentials to access the ballot
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Verify Your Identity</CardTitle>
          <CardDescription>
            Enter your username and password as provided by the elections office.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <Alert variant="error">{error}</Alert>
            )}

            <div>
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                type="text"
                placeholder="e.g., tfrazier"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div>
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="Enter your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <Button
              type="submit"
              className="w-full"
              size="lg"
              disabled={!username || !password || loading}
            >
              {loading ? 'Logging in...' : 'Access Ballot'}
            </Button>
          </form>

          <p className="text-sm text-center text-[var(--color-muted)] mt-4">
            Don&apos;t have an account?{' '}
            <Link to="/register" className="text-[var(--color-primary)] font-medium hover:underline">
              Register
            </Link>
          </p>
        </CardContent>
      </Card>

      <div className="mt-8 grid gap-4 sm:grid-cols-2">
        <Link to="/lookup" className="block">
          <Card className="h-full transition-colors hover:bg-gray-50">
            <CardContent className="flex items-center gap-3 p-4">
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-[var(--color-accent)]/10 text-[var(--color-primary)]">
                <SearchIcon />
              </div>
              <div>
                <p className="font-medium">Look Up Your Vote</p>
                <p className="text-sm text-[var(--color-muted)]">Verify your vote was recorded</p>
              </div>
            </CardContent>
          </Card>
        </Link>

        <Link to="/participation" className="block">
          <Card className="h-full transition-colors hover:bg-gray-50">
            <CardContent className="flex items-center gap-3 p-4">
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-[var(--color-accent)]/10 text-[var(--color-primary)]">
                <UsersIcon />
              </div>
              <div>
                <p className="font-medium">Check Participation</p>
                <p className="text-sm text-[var(--color-muted)]">See if a voter has voted</p>
              </div>
            </CardContent>
          </Card>
        </Link>
      </div>
    </div>
  );
}
