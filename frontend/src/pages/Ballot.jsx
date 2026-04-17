import React, { useEffect, useState } from 'react';
import { fetchBallot } from '../api/client';
import { Navigate, useNavigate } from 'react-router-dom';

export default function Ballot() {
  // Prevent access if not authenticated/signed up
  const auth = localStorage.getItem('voterAuth');
  if (!auth) {
    return <Navigate to="/login" replace />;
  }

  const [ballot, setBallot] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    let mounted = true;
    fetchBallot().then(data => {
      if (mounted) setBallot(data);
    }).catch((err) => {
      if (mounted) {
        setError(err.message);
        setBallot({ races: [] });
      }
    });
    return () => { mounted = false; };
  }, []);

  if (!ballot) return <div>Loading ballot...</div>;

  function handleProceedToReview() {
    // For now store a placeholder selections object so Review can be reached.
    // Real selection logic should replace this.
    const existing = localStorage.getItem('ballotSelections');
    if (!existing) {
      const selections = { selections: [] };
      localStorage.setItem('ballotSelections', JSON.stringify(selections));
    }
    // clear any previous 'reviewed' flag
    localStorage.removeItem('reviewed');
    navigate('/review');
  }

  return (
    <div>
      <h2>{ballot.electionName || 'Ballot'}</h2>
      {ballot.races && ballot.races.length === 0 && <p>No active races found.</p>}
      <ul>
        {(ballot.races || []).map((r) => (
          <li key={r.raceId}>{r.raceName}</li>
        ))}
      </ul>
      <div style={{ marginTop: 16 }}>
        <button onClick={handleProceedToReview}>Save selections &amp; Review</button>
      </div>
    </div>
  );
}
