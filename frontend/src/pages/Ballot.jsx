import React, { useEffect, useState } from 'react';
import { fetchBallot } from '../api/client';

export default function Ballot() {
  const [ballot, setBallot] = useState(null);

  useEffect(() => {
    let mounted = true;
    fetchBallot().then(data => {
      if (mounted) setBallot(data);
    }).catch(() => {
      if (mounted) setBallot({ races: [] });
    });
    return () => { mounted = false; };
  }, []);

  if (!ballot) return <div>Loading ballot...</div>;

  return (
    <div>
      <h2>Ballot</h2>
      {ballot.races && ballot.races.length === 0 && <p>No active races found.</p>}
      <ul>
        {(ballot.races || []).map((r) => (
          <li key={r.id}>{r.title}</li>
        ))}
      </ul>
    </div>
  );
}
