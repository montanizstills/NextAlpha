import { apiUrl, getApiBaseUrl } from './client';

const ORIGINAL = process.env.REACT_APP_API_BASE_URL;

afterEach(() => {
  if (ORIGINAL === undefined) {
    delete process.env.REACT_APP_API_BASE_URL;
  } else {
    process.env.REACT_APP_API_BASE_URL = ORIGINAL;
  }
});

test('defaults to same-origin when REACT_APP_API_BASE_URL is unset', () => {
  delete process.env.REACT_APP_API_BASE_URL;
  expect(getApiBaseUrl()).toBe('');
  expect(apiUrl('/health')).toBe('/health');
});

test('prefixes paths with REACT_APP_API_BASE_URL, trimming trailing slashes', () => {
  process.env.REACT_APP_API_BASE_URL = 'https://api.example.test/';
  expect(apiUrl('health')).toBe('https://api.example.test/health');
  expect(apiUrl('/api/products/all')).toBe(
    'https://api.example.test/api/products/all'
  );
});
