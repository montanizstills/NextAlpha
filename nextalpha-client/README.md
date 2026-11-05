# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Configuration

| Variable | Default | Purpose |
| --- | --- | --- |
| `REACT_APP_API_BASE_URL` | empty (same-origin) | Base URL of the NextAlpha API (`nextalpha-application`), e.g. `http://localhost:5000`. Trailing slashes are trimmed. Read by `src/api/client.js`; the app pings `GET /health` on load. |

Like all `REACT_APP_*` variables, it is inlined at **build time**, so set it before `npm start` / `npm run build` (e.g. in `.env.local` or the shell). Do not put secrets in it: it ends up in the public bundle. If you point it at a different origin, the API must allow that origin via CORS.

### Local development (same-origin via the dev proxy)

By default `REACT_APP_API_BASE_URL` is left **unset**, so the client calls the API on the same origin. In development, `npm start` forwards those calls to the backend through the CRA `"proxy"` field in `package.json`:

```json
"proxy": "http://localhost:5081"
```

`http://localhost:5081` is the `http` launch profile from `nextalpha-application/Properties/launchSettings.json`. Start the API with that profile (for example `dotnet run --launch-profile http`), then run `npm start`. A request such as `GET /health` from the page is forwarded to `http://localhost:5081/health`. If the backend's dev port changes, update `proxy` too.

The CRA proxy only forwards requests whose `Accept` header is not `text/html`. `src/api/client.js` sends `Accept: application/json`, so its requests are forwarded. The proxy exists only in `npm start`. A production build has to be served from the same origin as the API, or built with `REACT_APP_API_BASE_URL` set.

## Toolchain notes

- This app uses `react-scripts` 2.x (webpack 4, Jest 23). Webpack 4 needs Node's legacy OpenSSL provider on Node 17+, so `start` and `build` pass `--openssl-legacy-provider` to Node. Verified on Node 22.
- `react-scripts` 2.x uses the classic JSX transform: every file that contains JSX must `import React from 'react'`.
- Jest 23's jsdom has no `MutationObserver`, so Testing Library's `findBy*` / `waitFor` do not work; flush async effects with `await act(async () => { render(...) })` instead (see `src/App.test.js`).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can't go back!**

If you aren't satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you're on your own.

You don't have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn't feel obligated to use this feature. However we understand that this tool wouldn't be useful if you couldn't customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: [https://facebook.github.io/create-react-app/docs/code-splitting](https://facebook.github.io/create-react-app/docs/code-splitting)

### Analyzing the Bundle Size

This section has moved here: [https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size](https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size)

### Making a Progressive Web App

This section has moved here: [https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app](https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app)

### Advanced Configuration

This section has moved here: [https://facebook.github.io/create-react-app/docs/advanced-configuration](https://facebook.github.io/create-react-app/docs/advanced-configuration)

### Deployment

This section has moved here: [https://facebook.github.io/create-react-app/docs/deployment](https://facebook.github.io/create-react-app/docs/deployment)

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)
