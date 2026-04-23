import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { fetchBallot } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { RadioOption } from '../components/ui/RadioGroup';
import { Progress } from '../components/ui/Progress';
import { Alert } from '../components/ui/Alert';

export default function Ballot() {
  const navigate = useNavigate();
  const { currentVoter, hasVoted, selections, setSelection, setElectionId } = useVoting();

  const [ballot, setBallot] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!currentVoter) {
      navigate('/');
      return;
    }

    let mounted = true;
    fetchBallot()
      .then((data) => {
        if (mounted) {
          setBallot(data);
          setElectionId(data.electionId);
          setLoading(false);
        }
      })
      .catch((err) => {
        if (mounted) {
          setError(err.message);
          setLoading(false);
        }
      });
    return () => { mounted = false; };
  }, [currentVoter, navigate, setElectionId]);

  if (!currentVoter) return null;

  if (hasVoted) {
    return (
      <div className="max-w-2xl mx-auto">
        <Alert variant="info">
          You have already voted in this election. Each voter may only cast one ballot.
        </Alert>
        <div className="mt-4 text-center">
          <Button onClick={() => navigate('/confirmation')}>View Confirmation</Button>
        </div>
      </div>
    );
  }

  if (loading) return <p className="text-center text-[var(--color-muted)]">Loading ballot...</p>;
  if (error) return <Alert variant="error">{error}</Alert>;
  if (!ballot) return null;

  const races = ballot.races || [];
  const totalItems = races.length;
  const completedItems = Object.keys(selections).length;
  const progressPct = totalItems > 0 ? (completedItems / totalItems) * 100 : 0;

  return (
    <div className="max-w-3xl mx-auto">
      <div className="mb-8">
        <h2 className="text-2xl font-bold mb-1">Official Ballot</h2>
        <p className="text-[var(--color-muted)]">{ballot.electionName}</p>

        <div className="mt-4 p-4 bg-[var(--color-card)] rounded-lg border border-[var(--color-border)]">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm font-medium">
              {completedItems} of {totalItems} races completed
            </span>
            <span className="text-sm text-[var(--color-muted)]">
              {Math.round(progressPct)}%
            </span>
          </div>
          <Progress value={progressPct} />
        </div>
      </div>

      <div className="space-y-6">
        {races.map((race) => (
          <Card key={race.raceId}>
            <CardHeader className="bg-[var(--color-accent)]/30">
              <div className="flex items-center justify-between">
                <CardTitle>{race.raceName}</CardTitle>
                {selections[race.raceId] && (
                  <span className="text-[var(--color-success)] text-sm font-medium">Selected</span>
                )}
              </div>
              <CardDescription>
                {race.raceType === 'YesNo' ? 'Vote yes or no' : 'Vote for one candidate'}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                {race.choices.map((choice) => (
                  <RadioOption
                    key={choice.choiceId}
                    name={`race-${race.raceId}`}
                    value={String(choice.choiceId)}
                    checked={selections[race.raceId] === String(choice.choiceId)}
                    onChange={(val) => setSelection(race.raceId, val)}
                    label={choice.choiceName}
                    description={choice.party || null}
                  />
                ))}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="mt-8 flex justify-end">
        <Button size="lg" onClick={() => navigate('/review')}>
          Review Selections
        </Button>
      </div>
    </div>
  );
}
