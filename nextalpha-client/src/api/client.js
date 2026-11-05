// Minimal API helper. Base URL comes from REACT_APP_API_BASE_URL (inlined at
// build time by react-scripts). When unset, requests are same-origin.
export function getApiBaseUrl() {
  const raw = process.env.REACT_APP_API_BASE_URL || '';
  return raw.replace(/\/+$/, '');
}

export function apiUrl(path) {
  const suffix = path.startsWith('/') ? path : `/${path}`;
  return `${getApiBaseUrl()}${suffix}`;
}

export async function apiGet(path, options = {}) {
  const response = await fetch(apiUrl(path), {
    ...options,
    method: 'GET',
    headers: { Accept: 'application/json', ...(options.headers || {}) },
  });
  if (!response.ok) {
    throw new Error(`GET ${path} failed with status ${response.status}`);
  }
  const contentType = response.headers.get('content-type') || '';
  return contentType.includes('application/json')
    ? response.json()
    : response.text();
}

export function getHealth() {
  return apiGet('/health');
}
