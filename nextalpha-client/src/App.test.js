import React from 'react';
import { act, render, screen } from '@testing-library/react';
import App from './App';

// Jest 23's jsdom lacks MutationObserver, so findBy*/waitFor are unavailable;
// an async act() flushes the pending fetch promise and resulting state update.
async function renderApp() {
  await act(async () => {
    render(<App />);
  });
}

function mockFetch(impl) {
  global.fetch = jest.fn(impl);
}

afterEach(() => {
  delete global.fetch;
});

test('renders the greeting and reports a healthy API', async () => {
  mockFetch(() =>
    Promise.resolve({
      ok: true,
      status: 200,
      headers: { get: () => 'text/plain' },
      text: () => Promise.resolve('Healthy'),
    })
  );

  await renderApp();

  expect(screen.getByText(/hello, world!/i)).toBeInTheDocument();
  expect(screen.getByText('API status: ok')).toBeInTheDocument();
  expect(global.fetch).toHaveBeenCalledWith('/health', expect.any(Object));
});

test('reports the API as unavailable when /health fails', async () => {
  mockFetch(() => Promise.reject(new Error('network down')));

  await renderApp();

  expect(screen.getByText('API status: unavailable')).toBeInTheDocument();
});
