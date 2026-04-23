import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { lookupVote } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input, Label } from '../components/ui/Input';
import { Alert } from '../components/ui/Alert';

export default function VoteLookup() {
  const navigate = useNavigate();
  const [code, setCode] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setResult(null);
    setLoading(true);

    try {
      const data = await lookupVote(code);
      setResult(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="mb-8 rounded-lg bg-[var(--color-primary)] p-6 text-white">
        <h2 className="text-2xl font-bold mb-1">Verify Your Vote</h2>
        <p className="opacity-80">Enter your confirmation code to see the selections that were recorded.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Confirmation Code</CardTitle>
          <CardDescription>
            You received this after submitting your ballot. It looks like a long string of letters and numbers.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && <Alert variant="error">{error}</Alert>}

            <div>
              <Label htmlFor="code">Confirmation code</Label>
              <Input
                id="code"
                type="text"
                placeholder="e.g., 055b02cd-fd05-4b79-b341-11be0e6ae426"
                value={code}
                onChange={(e) => setCode(e.target.value)}
                autoComplete="off"
                spellCheck={false}
              />
            </div>

            <Button type="submit" className="w-full" size="lg" disabled={!code.trim() || loading}>
              {loading ? 'Looking up...' : 'Look Up My Vote'}
            </Button>
          </form>
        </CardContent>
      </Card>

      {result && (
        <div className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>Your Recorded Selections</CardTitle>
              <CardDescription>{result.electionName}</CardDescription>
            </CardHeader>
            <CardContent>
              {result.selections.length === 0 ? (
                <Alert variant="info">No selections were recorded for this code.</Alert>
              ) : (
                <ul className="divide-y divide-[var(--color-border)]">
                  {result.selections.map((item) => (
                    <li key={item.raceId} className="py-3 flex items-center justify-between">
                      <div>
                        <div className="text-sm text-[var(--color-muted)]">{item.raceName}</div>
                        <div className="font-medium">{item.candidateName}</div>
                      </div>
                      {item.party && (
                        <span className="text-sm text-[var(--color-muted)]">{item.party}</span>
                      )}
                    </li>
                  ))}
                </ul>
              )}
            </CardContent>
          </Card>
        </div>
      )}

      <div className="mt-6 text-center">
        <Button variant="outline" onClick={() => navigate('/')}>Back</Button>
      </div>
    </div>
  );
}
