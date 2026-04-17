import React from 'react';

export default function Submit() {
  function handleSubmit() {
    // TODO: send ballot to backend
    alert('Ballot submitted (placeholder)');
  }

  return (
    <div>
      <h2>Submit Ballot</h2>
      <p>Confirm and submit your ballot.</p>
      <button onClick={handleSubmit}>Submit</button>
    </div>
  );
}
