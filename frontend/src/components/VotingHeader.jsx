import { Link } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';

export default function VotingHeader() {
  const { currentVoter, logout } = useVoting();

  return (
    <header className="bg-[var(--color-primary)] text-[var(--color-primary-foreground)] border-b-4 border-[var(--color-accent)]">
      <div className="max-w-5xl mx-auto px-4 py-4 flex items-center justify-between">
        <div>
          <Link to="/" className="text-lg font-bold tracking-tight hover:opacity-90">
            Pacopolis Electronic Voting System
          </Link>
        </div>

        <div className="flex items-center gap-3">
          {currentVoter && (
            <>
              <span className="text-sm opacity-80">
                {currentVoter.firstName} {currentVoter.lastName}
              </span>
              <button
                onClick={logout}
                className="text-sm px-3 py-1 rounded border border-white/30 hover:bg-white/10"
              >
                Log Out
              </button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
