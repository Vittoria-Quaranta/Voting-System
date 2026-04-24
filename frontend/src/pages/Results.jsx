import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchResults } from '../api/client';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Progress } from '../components/ui/Progress';
import { Alert } from '../components/ui/Alert';

export default function Results() {
  const navigate = useNavigate();
  const [results, setResults] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchResults()
      .then((data) => {
        setResults(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <p className="text-center text-[var(--color-muted)]">Loading results...</p>;

  if (error) {
    return (
      <div className="max-w-2xl mx-auto">
        <Alert variant="info">{error}</Alert>
        <div className="mt-4 text-center">
          <Button variant="outline" onClick={() => navigate('/')}>Back</Button>
        </div>
      </div>
    );
  }

  if (!results) return null;

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-8 rounded-lg bg-[var(--color-primary)] p-6 text-white">
        <h2 className="text-2xl font-bold mb-1">Election Results</h2>
        <p className="opacity-80">{results.electionName}</p>
      </div>

      <div className="space-y-6">
        {results.races.map((race) => (
          <Card key={race.raceId}>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle>{race.raceName}</CardTitle>
                <span className="text-sm text-[var(--color-muted)]">
                  {race.totalVotes.toLocaleString()} total votes
                </span>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {race.candidates.map((candidate) => (
                  <div key={candidate.candidateId} className="space-y-1">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <span className="font-medium">{candidate.candidateName}</span>
                        {candidate.party && (
                          <span className="text-sm text-[var(--color-muted)]">({candidate.party})</span>
                        )}
                        {candidate.isWinner && (
                          <span className="text-xs font-semibold px-2 py-0.5 rounded-full bg-[var(--color-success)] text-white">
                            Winner
                          </span>
                        )}
                      </div>
                      <div className="text-right">
                        <span className="font-semibold">{candidate.votes.toLocaleString()}</span>
                        <span className="text-sm text-[var(--color-muted)] ml-2">({candidate.percentage}%)</span>
                      </div>
                    </div>
                    <Progress
                      value={candidate.percentage}
                      className={candidate.isWinner ? '[&>div]:bg-[var(--color-success)]' : ''}
                    />
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="mt-8 text-center">
        <Button variant="outline" onClick={() => navigate('/')}>Back</Button>
      </div>
    </div>
  );
}
