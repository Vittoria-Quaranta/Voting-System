import React from 'react';
import { BrowserRouter, Routes, Route, NavLink } from 'react-router-dom';
import Login from './pages/Login';
import Ballot from './pages/Ballot';
import Review from './pages/Review';
import Submit from './pages/Submit';
import VoteLookup from './pages/VoteLookup';
import ParticipationCheck from './pages/ParticipationCheck';
import AdminResults from './pages/AdminResults';

function App() {
  // Common style for nav items
  const navLinkStyle = ({ isActive }) => ({
    textDecoration: 'none',
    color: isActive ? '#000' : '#666',
    fontWeight: isActive ? '600' : '400',
    fontSize: '14px',
    padding: '8px 12px',
    borderRadius: '6px',
    transition: 'background 0.2s',
    backgroundColor: isActive ? '#f0f0f0' : 'transparent',
  });

  return (
    <BrowserRouter>
      <header style={{
        padding: '16px 24px',
        borderBottom: '1px solid #eaeaea',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between'
      }}>
        <div style={{ fontWeight: 'bold', fontSize: '18px' }}>VoterPortal</div>
        <nav style={{ display: 'flex', gap: '8px' }}>
          {/* Combined Home/Login into one destination */}
          <NavLink style={navLinkStyle} to="/login">Login / Sign Up</NavLink>
          <NavLink style={navLinkStyle} to="/ballot">Ballot</NavLink>
          <NavLink style={navLinkStyle} to="/review">Review</NavLink>
          <NavLink style={navLinkStyle} to="/submit">Submit</NavLink>
          <NavLink style={navLinkStyle} to="/lookup">My Vote</NavLink>
          <NavLink style={navLinkStyle} to="/participation">Participation</NavLink>
          <NavLink style={navLinkStyle} to="/admin/results">Results</NavLink>
        </nav>
      </header>

      <main style={{ padding: '24px', maxWidth: '1200px', margin: '0 auto' }}>
        <Routes>
          {/* Both root and login now point to the same component */}
          <Route path="/" element={<Login />} />
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
