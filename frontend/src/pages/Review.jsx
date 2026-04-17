import React from 'react';
import { Navigate, useNavigate } from 'react-router-dom';

// The Review page requires that the user has saved ballot selections first.

export default function Review() {
  const selectionsRaw = localStorage.getItem('ballotSelections');
  const navigate = useNavigate();
  if (!selectionsRaw) {
    return <Navigate to="/ballot" replace />;
  }

  const selections = JSON.parse(selectionsRaw);

  function proceedToSubmit() {
    // mark that review was completed
    localStorage.setItem('reviewed', '1');
    navigate('/submit');
  }

  return (
    <div>
      <h2>Review Selections</h2>
      <p>Show current selections and allow edits before submission.</p>
      <pre style={{ background: '#f7f7f7', padding: 12 }}>{JSON.stringify(selections, null, 2)}</pre>
      <div style={{ marginTop: 12 }}>
        <button onClick={() => navigate('/ballot')}>Edit Ballot</button>{' '}
        <button onClick={proceedToSubmit}>Proceed to Submit</button>
      </div>
    </div>
  );
}
