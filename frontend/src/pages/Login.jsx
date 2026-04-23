import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { authenticate } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input, Label } from '../components/ui/Input';
import { Alert } from '../components/ui/Alert';

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
        </CardContent>
      </Card>
    </div>
  );
}
