import { useState } from 'react';
import './Dashboard.css';
import { authenticate } from '../api/client';

const mockData = [
  { status: 'Accepted', voter: 'alex@example.com', poll: 'Election 2026', time: 'Apr 16, 2:00 PM' },
  { status: 'Rejected', voter: 'sam@example.com', poll: 'Budget Vote', time: 'Apr 16, 1:20 PM' },
  { status: 'Pending', voter: 'priya@example.com', poll: 'Club Voting', time: 'Apr 16, 12:45 PM' },
];

const statusColors = {
  Accepted: 'success',
  Rejected: 'danger',
  Pending: 'info',
};

export default function Dashboard() {
  const [filter, setFilter] = useState('All');
  const [search, setSearch] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const filteredData = mockData.filter((item) => {
    const matchesFilter = filter === 'All' || item.status === filter;
    const matchesSearch =
      item.voter.toLowerCase().includes(search.toLowerCase()) ||
      item.poll.toLowerCase().includes(search.toLowerCase());

    return matchesFilter && matchesSearch;
  });

  async function handleLogin(e) {
    e.preventDefault();
    setMessage('');
    setLoading(true);
    try {
      const res = await authenticate(username, password);
      setMessage(res.message || 'Login successful');
    } catch (err) {
      setMessage(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="dashboard">
      <div className="split">
        <aside className="signin-panel">
          <h2>Sign in</h2>
          <form onSubmit={handleLogin}>
            <div style={{ marginBottom: 12 }}>
              <label>Username</label>
              <br />
              <input
                className="search"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="you@example.com"
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label>Password</label>
              <br />
              <input
                className="search"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
              />
            </div>
            <button type="submit" disabled={loading || !username || !password} className="filter active">
              {loading ? 'Signing in...' : 'Sign in'}
            </button>
          </form>
          {message && <p style={{ marginTop: 12 }}>{message}</p>}
        </aside>

        <section className="table-panel">
          <div className="dashboard-header">
            <input
              className="search"
              placeholder="Search by voter or poll..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />

            <div className="filters">
              {['All', 'Accepted', 'Rejected', 'Pending'].map((f) => (
                <span
                  key={f}
                  className={`filter ${filter === f ? 'active' : ''}`}
                  onClick={() => setFilter(f)}
                >
                  {f}
                </span>
              ))}
            </div>
          </div>

          <table className="table">
            <thead>
              <tr>
                <th>Status</th>
                <th>Voter</th>
                <th>Poll</th>
                <th>Time</th>
              </tr>
            </thead>
            <tbody>
              {filteredData.map((item, index) => (
                <tr key={index}>
                  <td>
                    <span className={`badge ${statusColors[item.status]}`}>
                      {item.status}
                    </span>
                  </td>
                  <td>{item.voter}</td>
                  <td>{item.poll}</td>
                  <td>{item.time}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      </div>
    </div>
  );
}
