import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Alert } from '../components/ui/Alert';

export default function Confirmation() {
  const navigate = useNavigate();
  const { currentVoter, hasVoted, confirmationCode, logout } = useVoting();
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (!currentVoter) {
      navigate('/');
      return;
    }
    if (!hasVoted) {
      navigate('/ballot');
    }
  }, [currentVoter, hasVoted, navigate]);

  if (!currentVoter || !hasVoted) return null;

  async function handleCopy() {
    await navigator.clipboard.writeText(confirmationCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  function handleLogout() {
    logout();
    navigate('/');
  }

  return (
    <div className="max-w-md mx-auto text-center">
      <div className="mb-8">
        <div className="mx-auto mb-4 w-16 h-16 rounded-full bg-green-100 flex items-center justify-center">
          <span className="text-3xl text-[var(--color-success)]">&#10003;</span>
        </div>
        <h2 className="text-2xl font-bold mb-2">
          {confirmationCode ? 'Vote Successfully Cast' : 'You Have Already Voted'}
        </h2>
        <p className="text-[var(--color-muted)]">
          Thank you for participating, {currentVoter.firstName}!
        </p>
      </div>

      {confirmationCode ? (
        <Card>
          <CardHeader>
            <CardTitle>Your Confirmation Code</CardTitle>
            <CardDescription>Save this code to verify your vote later</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-center gap-2 bg-gray-100 rounded-lg p-4 mb-3">
              <code className="text-lg font-mono font-bold tracking-wider">
                {confirmationCode}
              </code>
              <button
                onClick={handleCopy}
                className="text-sm text-[var(--color-muted)] hover:text-gray-900 px-2 py-1 rounded border border-[var(--color-border)]"
                title="Copy to clipboard"
              >
                Copy
              </button>
            </div>
            {copied && (
              <p className="text-sm text-[var(--color-success)]">Copied to clipboard!</p>
            )}
          </CardContent>
        </Card>
      ) : (
        <Alert variant="info">
          You have already voted in this election. Use your confirmation code
          from your original vote to verify it was recorded.
        </Alert>
      )}

      <div className="mt-8 space-y-3">
        <Button variant="outline" className="w-full" onClick={() => navigate('/lookup')}>
          Verify Your Vote
        </Button>
        <Button variant="outline" className="w-full" onClick={() => navigate('/results')}>
          View Results
        </Button>
        <Button variant="ghost" className="w-full" onClick={handleLogout}>
          Log Out
        </Button>
      </div>
    </div>
  );
}
