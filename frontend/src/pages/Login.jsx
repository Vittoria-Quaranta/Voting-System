import React, { useState } from 'react';

export default function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    function handleSubmit(e) {
        e.preventDefault();
        // TODO: call backend authentication
        alert(`Login attempted for ${username}`);
    }

    return (
        <div>
            <h2>Voter Login</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Username</label>
                    <br />
                    <input
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </div>

                <div>
                    <label>Password</label>
                    <br />
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button type="submit" disabled={!username || !password}>
                    Login
                </button>
            </form>
        </div>
    );
}