import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import Login from './pages/Login';
import Ballot from './pages/Ballot';
import Review from './pages/Review';
import Submit from './pages/Submit';
import VoteLookup from './pages/VoteLookup';
import ParticipationCheck from './pages/ParticipationCheck';
import AdminResults from './pages/AdminResults';

function App() {
  return (
    <BrowserRouter>
      <header style={{ padding: 12, borderBottom: '1px solid #ddd' }}>
        <nav style={{ display: 'flex', gap: 12 }}>
          <Link to="/">Home</Link>
          <Link to="/login">Login</Link>
          <Link to="/ballot">Ballot</Link>
          <Link to="/review">Review</Link>
          <Link to="/submit">Submit</Link>
          <Link to="/lookup">My Vote</Link>
          <Link to="/participation">Participation</Link>
          <Link to="/admin/results">Results</Link>
        </nav>
      </header>

      <main style={{ padding: 12 }}>
        <Routes>
          <Route path="/" element={<div><h1>Pacopolis Voting System</h1><p>Welcome to the voting frontend.</p></div>} />
          <Route path="/login" element={<Login />} />
          <Route path="/ballot" element={<Ballot />} />
          <Route path="/review" element={<Review />} />
          <Route path="/submit" element={<Submit />} />
          <Route path="/lookup" element={<VoteLookup />} />
          <Route path="/participation" element={<ParticipationCheck />} />
          <Route path="/admin/results" element={<AdminResults />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;
