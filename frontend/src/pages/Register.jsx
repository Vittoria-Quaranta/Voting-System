import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useVoting } from '../context/VotingContext';
import { registerVoter, authenticate } from '../api/client';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input, Label } from '../components/ui/Input';
import { Alert } from '../components/ui/Alert';

const MIN_PASSWORD_LENGTH = 8;

export default function Register() {
  const navigate = useNavigate();
  const { setCurrentVoter, markVoted } = useVoting();

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    if (password.length < MIN_PASSWORD_LENGTH) {
      setError(`Password must be at least ${MIN_PASSWORD_LENGTH} characters.`);
      return;
    }
    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setLoading(true);
    try {
      await registerVoter({
        firstName,
        lastName,
        username,
        password,
        dateOfBirth: dateOfBirth || null,
      });

      // auto-login so the voter lands straight on the ballot
      const loginData = await authenticate(username, password);
      setCurrentVoter({
        voterId: loginData.voterId,
        firstName: loginData.firstName,
        lastName: loginData.lastName,
      });
      if (loginData.hasVoted) {
        markVoted(null);
        navigate('/confirmation');
      } else {
        navigate('/ballot');
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  const canSubmit = firstName && lastName && username && password && confirmPassword && !loading;

  return (
    <div className="max-w-md mx-auto">
      <div className="text-center mb-8">
        <h2 className="text-2xl font-bold mb-2">Create Your Account</h2>
        <p className="text-[var(--color-muted)]">
          Register to cast your vote in the active election.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Voter Registration</CardTitle>
          <CardDescription>
            Provide your name, a username, and a password to create an account.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && <Alert variant="error">{error}</Alert>}

            <div className="grid grid-cols-2 gap-3">
              <div>
                <Label htmlFor="firstName">First name</Label>
                <Input
                  id="firstName"
                  type="text"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  autoComplete="given-name"
                />
              </div>
              <div>
                <Label htmlFor="lastName">Last name</Label>
                <Input
                  id="lastName"
                  type="text"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  autoComplete="family-name"
                />
              </div>
            </div>

            <div>
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                autoComplete="username"
              />
            </div>

            <div>
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                autoComplete="new-password"
              />
              <p className="text-xs text-[var(--color-muted)] mt-1">
                At least {MIN_PASSWORD_LENGTH} characters.
              </p>
            </div>

            <div>
              <Label htmlFor="confirmPassword">Confirm password</Label>
              <Input
                id="confirmPassword"
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                autoComplete="new-password"
              />
            </div>

            <div>
              <Label htmlFor="dateOfBirth">Date of birth (optional)</Label>
              <Input
                id="dateOfBirth"
                type="date"
                value={dateOfBirth}
                onChange={(e) => setDateOfBirth(e.target.value)}
              />
            </div>

            <Button type="submit" className="w-full" size="lg" disabled={!canSubmit}>
              {loading ? 'Creating account...' : 'Create Account'}
            </Button>
          </form>

          <p className="text-sm text-center text-[var(--color-muted)] mt-4">
            Already have an account?{' '}
            <Link to="/login" className="text-[var(--color-primary)] font-medium hover:underline">
              Log in
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
