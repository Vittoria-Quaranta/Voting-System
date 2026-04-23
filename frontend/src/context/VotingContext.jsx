import { createContext, useContext, useState, useCallback } from 'react';

const VotingContext = createContext(undefined);

export function VotingProvider({ children }) {
  const [currentVoter, setCurrentVoter] = useState(null);
  const [electionId, setElectionId] = useState(null);
  const [selections, setSelections] = useState({});
  const [hasVoted, setHasVoted] = useState(false);
  const [confirmationCode, setConfirmationCode] = useState(null);

  const setSelection = useCallback((raceId, choiceId) => {
    setSelections((prev) => ({ ...prev, [raceId]: choiceId }));
  }, []);

  const clearSelections = useCallback(() => {
    setSelections({});
  }, []);

  const markVoted = useCallback((code) => {
    setConfirmationCode(code);
    setHasVoted(true);
  }, []);

  const logout = useCallback(() => {
    setCurrentVoter(null);
    setElectionId(null);
    setSelections({});
    setHasVoted(false);
    setConfirmationCode(null);
  }, []);

  return (
    <VotingContext.Provider
      value={{
        currentVoter,
        setCurrentVoter,
        electionId,
        setElectionId,
        selections,
        setSelection,
        clearSelections,
        hasVoted,
        confirmationCode,
        markVoted,
        logout,
      }}
    >
      {children}
    </VotingContext.Provider>
  );
}

export function useVoting() {
  const context = useContext(VotingContext);
  if (context === undefined) {
    throw new Error('useVoting must be used within a VotingProvider');
  }
  return context;
}
