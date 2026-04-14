import React, { useState } from 'react';

export default function Login() {
  const [voterId, setVoterId] = useState('');
  const [pin, setPin] = useState('');

  function handleSubmit(e) {
    e.preventDefault();
    // TODO: call backend authentication
    alert(`Login attempted for ${voterId}`);
  }

  return (
    <div>
      <h2>Voter Login</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Voter ID</label>
          <br />
          <input value={voterId} onChange={(e) => setVoterId(e.target.value)} />
        </div>
        <div>
          <label>PIN</label>
          <br />
          <input type="password" value={pin} onChange={(e) => setPin(e.target.value)} />
        </div>
        <button type="submit">Login</button>
      </form>
    </div>
  );
}
