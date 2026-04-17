import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authenticate } from '../api/client';
// This import will now work because the 'assets' folder exists
import pacopolisCity from '../assets/pacopolis.webp'; 

export default function Login() {
  const [isLogin, setIsLogin] = useState(true);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const data = await authenticate(username, password);
      localStorage.setItem('voterAuth', JSON.stringify(data));
      navigate('/ballot');
    } catch (err) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={containerStyle}>
      {/* LEFT SIDE: PACPOLIS BACKGROUND & ANIMATION */}
      <div style={{
        ...animationWrapperStyle,
        backgroundImage: `linear-gradient(rgba(0, 48, 73, 0.7), rgba(0, 48, 73, 0.7)), url(${pacopolisCity})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center'
      }}>
        {/* REQUESTED TEXT: BOLD & SPACED */}
        <div style={textOverlayStyle}>
          <h2 style={headlineStyle}>PACOPOLIS <br/> VOTING SYSTEM</h2>
          <p style={subheadlineStyle}>YOUR VOICE MATTERS</p>
        </div>

        {/* BOX SLOT & BALLOT */}
        <svg width="400" height="400" viewBox="0 0 400 400" fill="none" style={{ marginTop: '100px' }}>
          {/* THE SLOT FRAME */}
          <path d="M60 220 L60 350 L340 350 L340 220" stroke="#FDF0D5" strokeWidth="4" strokeLinecap="round"/>
          <line x1="60" y1="220" x2="340" y2="220" stroke="#001F3D" strokeWidth="16" strokeLinecap="round"/>

          {/* THE ANIMATED BALLOT */}
          <g className="ballot-slide">
            <rect x="140" y="40" width="120" height="160" fill="#FDF0D5" rx="4" />
            <line x1="160" y1="80" x2="240" y2="80" stroke="#8A9AA9" strokeWidth="3" />
            <line x1="160" y1="110" x2="240" y2="110" stroke="#8A9AA9" strokeWidth="3" />
          </g>
        </svg>

        <style>{`\n          @keyframes slideIn {\n            0% { transform: translateY(-220px); opacity: 0; }\n            20% { transform: translateY(-110px); opacity: 1; }\n            60% { transform: translateY(140px); opacity: 0; }\n            100% { transform: translateY(-220px); opacity: 0; }\n          }\n          .ballot-slide {\n            animation: slideIn 4.5s infinite cubic-bezier(0.45, 0.05, 0.55, 0.95);\n          }\n        `}</style>
      </div>

      {/* RIGHT SIDE: AUTHENTICATION FORM */}
      <div style={formSectionStyle}>
        <div style={{ maxWidth: '340px', width: '100%' }}>
          <h1 style={{ color: '#003049', fontSize: '2.5rem', fontWeight: '800', marginBottom: '10px' }}>
            {isLogin ? 'Voter Login' : 'Register'}
          </h1>
          <p style={{ color: '#666', marginBottom: '40px' }}>
            Secure electronic access for Pacopolis citizens.
          </p>
          
          <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <input style={inputStyle} type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
            <input style={inputStyle} type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <button style={primaryButtonStyle} type="submit" disabled={loading}>{loading ? 'Signing in...' : (isLogin ? 'Sign In' : 'Create Account')}</button>
            {error && <div style={{ color: 'salmon' }}>{error}</div>}
          </form>
          
          <button onClick={() => setIsLogin(!isLogin)} style={secondaryButtonStyle}>
            {isLogin ? "Need to register? Click here" : "Already have an ID? Login"}
          </button>
        </div>
      </div>
    </div>
  );
}

// --- STYLES ---
const containerStyle = { display: 'flex', height: '100vh', backgroundColor: '#fff', overflow: 'hidden', fontFamily: 'system-ui, sans-serif' };

const animationWrapperStyle = { 
  flex: 1.4, 
  display: 'flex', 
  flexDirection: 'column', 
  alignItems: 'center', 
  justifyContent: 'center', 
  position: 'relative' 
};

const textOverlayStyle = { 
  position: 'absolute', 
  top: '10%', 
  left: '8%', 
  color: '#FDF0D5' 
};

const headlineStyle = { 
  fontSize: '48px', 
  fontWeight: '900', 
  lineHeight: '0.9', 
  margin: 0,
  letterSpacing: '-1px'
};

const subheadlineStyle = { 
  fontSize: '20px', 
  fontWeight: '700', 
  marginTop: '15px', 
  letterSpacing: '1px',
  opacity: 0.9
};

const formSectionStyle = { flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '60px', borderLeft: '1px solid #eee' };
const inputStyle = { width: '100%', padding: '16px 0', border: 'none', borderBottom: '2px solid #ddd', outline: 'none', fontSize: '16px' };
const primaryButtonStyle = { width: '100%', padding: '16px', backgroundColor: '#003049', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' };
const secondaryButtonStyle = { background: 'none', border: 'none', color: '#666', marginTop: '30px', cursor: 'pointer', textDecoration: 'underline' };
