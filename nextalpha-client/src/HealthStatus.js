import React, { useEffect, useState } from 'react';
import { getHealth } from './api/client';

// Placeholder that pings the backend's GET /health endpoint.
function HealthStatus() {
  const [status, setStatus] = useState('checking');

  useEffect(() => {
    let cancelled = false;
    getHealth()
      .then(() => {
        if (!cancelled) setStatus('ok');
      })
      .catch(() => {
        if (!cancelled) setStatus('unavailable');
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return <p data-testid="api-health">API status: {status}</p>;
}

export default HealthStatus;
