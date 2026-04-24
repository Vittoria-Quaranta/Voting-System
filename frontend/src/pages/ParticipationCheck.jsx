import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { checkParticipation } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input, Label } from '../components/ui/Input';
import { Alert } from '../components/ui/Alert';

export default function ParticipationCheck() {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setResult(null);
    setLoading(true);

    try {
      const data = await checkParticipation(username);
      setResult({ username: username.trim(), voted: data.voted });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="mb-8 rounded-lg bg-[var(--color-primary)] p-6 text-white">
        <h2 className="text-2xl font-bold mb-1">Voter Participation Check</h2>
        <p className="opacity-80">
          Look up whether a voter has participated in the active election. No ballot choices are revealed.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Voter Username</CardTitle>
          <CardDescription>
            Enter the voter&apos;s username. Only yes-or-no participation status will be returned.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && <Alert variant="error">{error}</Alert>}

            <div>
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                type="text"
                placeholder="e.g., tfrazier"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                autoComplete="off"
                spellCheck={false}
              />
            </div>

            <Button type="submit" className="w-full" size="lg" disabled={!username.trim() || loading}>
              {loading ? 'Checking...' : 'Check Participation'}
            </Button>
          </form>
        </CardContent>
      </Card>

      {result && (
        <div className="mt-8">
          <Alert variant={result.voted ? 'success' : 'info'}>
            <span className="font-semibold">{result.username}</span>{' '}
            {result.voted ? 'has voted in the active election.' : 'has not voted in the active election.'}
          </Alert>
        </div>
      )}

      <div className="mt-6 text-center">
        <Button variant="outline" onClick={() => navigate('/')}>Back</Button>
      </div>
    </div>
  );
}
