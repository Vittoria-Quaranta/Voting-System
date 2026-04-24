import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { fetchBallot, submitBallot } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Alert } from '../components/ui/Alert';

export default function Review() {
  const navigate = useNavigate();
  const { currentVoter, hasVoted, electionId, selections, markVoted } = useVoting();

  const [ballot, setBallot] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!currentVoter) {
      navigate('/');
      return;
    }
    if (hasVoted) {
      navigate('/confirmation');
      return;
    }

    fetchBallot().then(setBallot).catch(() => {});
  }, [currentVoter, hasVoted, navigate]);

  if (!currentVoter || hasVoted || !ballot) return null;

  const races = ballot.races || [];
  const totalItems = races.length;
  const completedItems = Object.keys(selections).length;
  const hasSkipped = completedItems < totalItems;

  function getSelectedChoice(race) {
    const selectedId = selections[race.raceId];
    if (!selectedId) return null;
    return race.choices.find((c) => String(c.choiceId) === selectedId);
  }

  async function handleSubmit() {
    setSubmitting(true);
    setError(null);

    try {
      const payload = {
        voterId: currentVoter.voterId,
        electionId,
        selections: Object.entries(selections).map(([raceId, choiceId]) => ({
          raceId: Number(raceId),
          choiceId: Number(choiceId),
        })),
      };

      const result = await submitBallot(payload);
      markVoted(result.confirmationCode);
      navigate('/confirmation');
    } catch (err) {
      // if backend says already voted, mark it and redirect
      if (err.message.toLowerCase().includes('already voted')) {
        markVoted(null);
        navigate('/confirmation');
        return;
      }
      setError(err.message);
      setSubmitting(false);
    }
  }

  return (
    <div className="max-w-3xl mx-auto">
      <div className="mb-8">
        <h2 className="text-2xl font-bold mb-1">Review Your Ballot</h2>
        <p className="text-[var(--color-muted)]">{ballot.electionName}</p>
      </div>

      {hasSkipped && (
        <Alert variant="warning" className="mb-6">
          You have not made selections for all races. Unselected races will not be counted.
        </Alert>
      )}

      {error && (
        <Alert variant="error" className="mb-6">{error}</Alert>
      )}

      <div className="space-y-4">
        {races.map((race) => {
          const selected = getSelectedChoice(race);
          return (
            <Card key={race.raceId}>
              <CardContent className="py-0">
                <div className="flex items-center justify-between py-4">
                  <div>
                    <h3 className="font-semibold">{race.raceName}</h3>
                    {selected ? (
                      <p className="text-sm mt-1">
                        <span className="font-medium">{selected.choiceName}</span>
                        {selected.party && (
                          <span className="text-[var(--color-muted)]"> ({selected.party})</span>
                        )}
                      </p>
                    ) : (
                      <p className="text-sm text-[var(--color-muted)] italic mt-1">No selection</p>
                    )}
                  </div>
                  <Button variant="ghost" size="sm" onClick={() => navigate('/ballot')}>
                    Edit
                  </Button>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <Card className="mt-8 border-[var(--color-primary)]">
        <CardHeader>
          <CardTitle>Ready to Submit?</CardTitle>
          <CardDescription>
            Once you submit your ballot, you cannot change your selections.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex gap-3 justify-end">
            <Button variant="outline" onClick={() => navigate('/ballot')}>
              Go Back
            </Button>
            <Button size="lg" onClick={handleSubmit} disabled={submitting}>
              {submitting ? 'Submitting...' : 'Submit Vote'}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
