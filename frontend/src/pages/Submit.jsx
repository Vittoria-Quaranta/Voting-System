import React, { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { submitBallot } from '../api/client';

export default function Submit() {
  const auth = localStorage.getItem('voterAuth');
  if (!auth) return <Navigate to="/login" replace />;

  const selectionsRaw = localStorage.getItem('ballotSelections');
  if (!selectionsRaw) return <Navigate to="/ballot" replace />;

  const reviewed = localStorage.getItem('reviewed');
  if (!reviewed) return <Navigate to="/review" replace />;

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  async function handleSubmit() {
    setLoading(true);
    setMessage('');
    try {
      const payload = { selections: JSON.parse(selectionsRaw) };
      const res = await submitBallot(payload);
      setMessage(res.message || 'Submission successful');
      // clear flow state
      localStorage.removeItem('ballotSelections');
      localStorage.removeItem('reviewed');
      navigate('/');
    } catch (err) {
      setMessage(err.message || 'Submission failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <h2>Submit Ballot</h2>
      <p>Confirm and submit your ballot.</p>
      <button onClick={handleSubmit} disabled={loading}>{loading ? 'Submitting...' : 'Submit'}</button>
      {message && <div style={{ marginTop: 12 }}>{message}</div>}
    </div>
  );
}
