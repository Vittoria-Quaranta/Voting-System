import React, { useEffect, useState } from 'react';

export default function AdminResults() {
  const [results, setResults] = useState(null);

  useEffect(() => {
    // TODO: call backend results endpoint
    setResults({ summary: 'No results (placeholder)' });
  }, []);

  return (
    <div>
      <h2>Election Results</h2>
      <pre>{JSON.stringify(results, null, 2)}</pre>
    </div>
  );
}
