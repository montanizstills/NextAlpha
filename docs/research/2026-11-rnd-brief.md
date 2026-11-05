# NextAlpha R&D Brief (November 2026)

- **Author:** Researcher (R&D) agent, for the Lead session
- **Date:** 2026-11-05
- **Branch reviewed:** `project-migration` @ `af0623b` ("Migrate NextAlpha from Google Drive dev/projects")
- **Status:** Proposal only. Nothing here was purchased, signed up for, deployed or published.

Prices and terms below were taken from public pages, or from third-party pages that cite them, between 2026-11-05 and the "checked" dates noted. They change often, so re-check them before any decision that spends money. Where the only source is a competitor's comparison page, the text says so.

---

## Executive summary

1. **The backend runs on an unsupported runtime.** .NET 7 went out of support on 2024-05-14. .NET 8 (LTS) and .NET 9 (STS) both reach end of support on **2026-11-10**, five days from now. **.NET 10 (LTS, supported to 2028-11-14)** is the only sensible target. .NET 11 (STS) ships on 2026-11-10 and is not worth chasing.
2. **The client toolchain is dead, and its globe dependency is an empty package.** CRA was deprecated in Feb 2025. The lockfile pins `react-scripts@2.1.3` (2019, webpack 4), which fails on modern Node. The locked `react-globe@2.0.1` is a **placeholder package with no code** ("just a placeholder for migration to a new owner"), so `<ReactGlobe />` cannot work as written. The newest real `react-globe` (5.0.2) dates from Aug 2020. Recommendation: rebuild the client on **Vite + React 19 + TypeScript** (Vite is at 8.3.x) and replace the globe with **`react-globe.gl`** (2.38.0, May 2026, peer `react: *`), or drop the globe.
3. **Both migrations are small because there is almost no code.** Backend: about 0.5–1 day. Client: about 1 day. Rebuilding is cheaper than upgrading.
4. **Market data licensing is the main constraint, ahead of price.** Every cheap or free tier surveyed is for personal or internal use only. Showing data to *anyone else* (even testers, for Massive) needs a business or display licence: roughly **$149/mo (Twelve Data Venture)** to **$2,499/mo (Massive Stocks Business)**, or Databento Plus at $1,750/mo for external distribution.
5. **Prototype data: Alpaca Basic (free)**, with Massive's free tier for EOD history as a fallback. Alpaca gives free real-time IEX quotes over WebSocket (30 symbols) and history back to 2016, but it **prohibits redistribution**, so the prototype must stay private to the owner.
6. **Production data: Twelve Data Venture ($149/mo, external display)** is the cheapest self-serve licence surveyed that allows a public display. **Massive Business** is the premium option if we need full SIP depth or options data. This decision needs human approval (cost plus legal terms).
7. **Stats engine: stay in .NET for v1.** Write returns, volatility, Sharpe, Sortino, max drawdown and t-tests directly on `MathNet.Numerics` 5.0.0 (stable but last released Apr 2022; 6.0 is still in beta). Accord.NET was archived in 2020, so avoid it. ML.NET (5.0.1) is for ML, not classical tests. Add a **Python sidecar (scipy 1.18 / statsmodels 0.15)** only when we need regressions, autocorrelation-robust tests or other econometrics. Keep the sidecar behind an internal HTTP contract so adding it later is cheap.
8. **"AI" v1 should narrate numbers that the code has already computed.** The LLM explains deterministic metrics and never produces figures itself. v1.1 adds natural language → a *structured* hypothesis spec (JSON validated against a schema), never free-form code. Keep the output impersonal, general, non-personalised and clearly labelled to stay within the publisher's-exclusion pattern. Avoid "AI-washing" claims (the SEC fined two advisers over these in 2024).
9. **Host compose-first on a free VM, and keep the setup portable.** Oracle Always Free still works, but **Ampere A1 was halved in mid-2026 to 2 OCPU / 12 GB**, and idle instances can be reclaimed. That is still enough for API + Postgres + client + optional sidecar. AWS EC2 + RDS at the smallest sensible size costs about **$30–35/mo** on-demand (new-account credits of up to $200 cover the first months).
10. **First backlog milestone:** move to .NET 10, rebuild the client on Vite, run everything with docker-compose and Postgres, and build a deterministic metrics API over cached EOD data. The steps that spend money or go public (paid data, LLM key, public URL) come after that, behind human approval.

---

## 1. Platform currency

### What the repo contains today (`project-migration`)

| Area | Finding |
|---|---|
| Backend | `nextalpha-application/NETWebAppMVC.csproj` targets `net7.0`. `ProductsController` is a placeholder with a `new BusinessLogic()` inline and no DI. Swagger is commented out. `ProductRepository` holds in-memory fruit data. There is no DB access yet. |
| Dockerfile | Uses `aspnet:7.0` / `sdk:7.0` images. Its `COPY ["NETWebAppMVC/NETWebAppMVC.csproj", ...]` paths don't match the folder layout (`nextalpha-application/`), so the build context is broken as committed. |
| Client | CRA layout (`react-scripts start`) with React 18.2. `react-scripts` resolves to **2.1.3** in the lockfile (Jan 2019, Babel 7.1, webpack 4). Testing-library and web-vitals packages are the CRA defaults. |
| Globe | `react-globe@2.0.1` is locked. The tarball contains only `package.json` (name + version) and a README saying it's a **placeholder for migration to a new owner**. It has no code, no `main` and no peer deps. The import in `App.js` cannot resolve to a component. |

### .NET

| Version | Type | End of support |
|---|---|---|
| .NET 7 | STS | **2024-05-14** (out of support for about 2.5 years) |
| .NET 8 | LTS | 2026-11-10 |
| .NET 9 | STS | 2026-11-10 (STS was extended to 24 months starting with .NET 9) |
| **.NET 10** | **LTS** | **2028-11-14** |
| .NET 11 | STS | Ships 2026-11-10 at .NET Conf. Expected to run about 24 months, ending around Nov 2028. |

**Recommendation:** target `net10.0`. It has the longest runway, and .NET 11 (STS) ends on about the same date. Re-check after .NET Conf (Nov 10–12) whether the 11.0 support date is officially published.

**Effort, about 0.5–1 day.**
- Change the TFM and the Dockerfile base images to `10.0`, and fix the Dockerfile paths.
- Move to DI registration (`AddScoped<IBusinessLogic, BusinessLogic>`).
- Swap the commented Swashbuckle setup for built-in `Microsoft.AspNetCore.OpenApi`, and add health checks.
- Delete the product sample.
- The 7→10 jump has few breaking changes for a project this small (minimal hosting already in use).

### Create React App

CRA was formally deprecated on **2025-02-14**. It had no active maintainers, broke on React 19 installs, and the React docs now point to frameworks (Next.js, React Router) or to Vite for build tooling. Vite is the closest match to a client-only CRA app. Vite 7 needs Node 20.19+ / 22.12+, and the current Vite is 8.3.x. React's current version is 19.3.

**Recommendation:** start a fresh `npm create vite@latest` project with React + TypeScript and copy in the (trivial) `App.js`. Add Vitest + Testing Library, and use a dev-server proxy to the API.

Consider Next.js or React Router framework mode only if we need SSR or SEO for public pages. A dashboard behind login doesn't need them, and SSR would also complicate the ASP.NET-serves-SPA option.

**Effort, about 1 day,** including Dockerfile/nginx for static hosting and CI build. Regard this as a **major dependency bump (React 18→19, CRA→Vite), so it needs human approval** under the working agreement. The diff, though, is essentially "new scaffold."

### react-globe

- The locked 2.0.1 is an empty placeholder, as described above.
- The real package (chrisrzhou) ran from 3.0.0 to **5.0.2, last published 2020-08-09**. Its peers are `react >= 16.13.1` and `three >= 0.118.3`, and it depends on an old Three.js glow/interaction add-on (`three.interaction`, `three-glow-mesh`).
- The npm package has been untouched since 2022 and gets about 2.2k downloads a month. React 18 should satisfy the peer range, but React 19 and current Three (0.187) are **untested and likely to break** through the old Three add-ons. Treat it as unmaintained.
- **Replacement:** `react-globe.gl` (vasturiano) is at 2.38.0 (published 2026-05-16), has peer `react: *`, about 1.4k GitHub stars, and is built on `globe.gl` with a large set of layers (points, arcs, hex bins, heatmaps).
- **Product question:** a globe is decorative for a market-analysis tool. A v1 could replace it with charts. For price charts, `lightweight-charts` (TradingView, Apache-2.0, v5.2.1) is the usual choice. Check its attribution requirement before use.

---

## 2. Market data providers

### Comparison (US equities focus)

| Provider | Asset coverage | Real-time / WS | Free tier | Paid entry | Display / redistribution |
|---|---|---|---|---|---|
| **Massive** (formerly Polygon.io, rebranded around Sep 2026) | Stocks, options, indices, FX, crypto, futures. Billed **per asset class**. | WebSockets on paid plans. Real-time all-US-exchange data from **Advanced $199/mo**. Lower tiers are delayed. | 5 calls/min, EOD data plus 15-min delayed, 2 years of history | Starter $29 / Developer $79 / Advanced $199 per month, per market, unlimited calls | Individual plans cover **only the subscriber's own use**. Showing data to anyone else (including testers, and including derived values) needs **Business: Stocks $2,499/mo** self-serve. Using it for an employer or LLC counts as "professional." |
| **Alpaca** (Market Data API) | US stocks, options, crypto | Free: real-time **IEX only**, WS limited to 30 symbols, SIP 15-min delayed. Algo Trader Plus: full SIP + OPRA, unlimited WS symbols. | 200 calls/min, history since 2016 | **$99/mo** Algo Trader Plus | The FAQ says Alpaca API data **cannot be redistributed**. Plan for private, personal use only. |
| **Twelve Data** | Stocks (US, EU and others), ETFs, FX, crypto, indicators | Real-time US. WS on Pro and above (500+ credits). Free/Grow WS works only on trial symbols. | 8 credits/min, 800/day, internal non-display use | Individual Grow $29. **Business Venture $149/mo** (610 credits/min, 500 WS credits, **external display**). Enterprise $1,099 for external distribution. | Clearest published display ladder. Individual plans are personal/internal only. Business is required for companies even for internal use. |
| **Finnhub** | Global stocks, fundamentals, news, FX, crypto | WS on the free tier (50 symbols) | 60 calls/min, US only. Candles are premium (free tier returns 403). | US market data from about $49.99/mo (billed quarterly) | **Self-serve plans are personal use only.** Commercial use needs an Enterprise contract (price on request). *Source: competitor (Massive) comparison page, checked 2026-10-23.* |
| **Alpha Vantage** | Stocks, FX, crypto, fundamentals, indicators | REST only. Real-time / 15-min US data needs a **separate entitlement** via its "Alpha X Terminal," described as personal use. | **25 requests/day** | $49.99/mo (75 req/min) up to $249.99 (1,200 req/min) | Display/commercial terms aren't stated on the premium page. You would need to contact them. |
| **Tiingo** | US/China equities EOD, IEX intraday, news, crypto, FX | IEX real-time WS. A multi-venue real-time API is in beta. | 50 req/hr, 1,000/day, 500 symbols/month | Power $30/mo (individual), $50/mo internal commercial | Internal use only. Redistribution is licensed separately. |
| **Databento** | Institutional-grade US equities, CME futures, options | Live data needs a subscription. Usage-based live pricing is not offered for US equities. | $125 of credits for new users (expire after 6 months), usage-based historical data per GB | **Standard $199/mo** (personal, 2 devices) | External distribution starts at **Plus, $1,750/mo** on an annual contract. Most datasets can be redistributed after 24 hours, subject to the publisher's licence. |

**IEX successors:** IEX Cloud shut down on 2024-08-31. Former executives bought the assets and launched **Blue Sky Data** with IEX-compatible APIs. It wasn't evaluated in depth because its current pricing and terms couldn't be confirmed from public pages.

### Licensing implications for NextAlpha

- **Raw quotes are licensed data.** Any public page that shows prices, or values *derived* from prices (Massive explicitly includes derived values), needs a display licence, usually with exchange fees passed through and pro/non-pro subscriber classification.
- **Consequence for scope:** while NextAlpha is private (owner-only), a free or individual tier is fine. A **public demo URL is the trigger** for a business licence. Delayed or EOD data lowers exchange-fee exposure but does not remove the need for a vendor display licence.
- **Mitigation for v1:** a public demo could show only **aggregates over the user's own uploaded data**, or synthetic data, until a licence is in place.

### Recommendation

- **Prototype: Alpaca Basic (free).** It has real-time IEX over WS, generous REST limits and 10 years of history. Pair it with **Massive's free tier** for EOD aggregates if Alpaca's IEX-only quotes become a problem. Keep the deployment private to the owner. *Creating any account or API key is a human step.*
- **Production: Twelve Data Business "Venture" ($149/mo).** It is the cheapest self-serve plan surveyed that explicitly allows external display, with REST + WS and real-time US/EU data. Move to **Massive Business** if we need consolidated SIP depth, options or tick-level data and the budget allows $2.5k/mo.
- Hide everything behind an **`IMarketDataProvider` abstraction** so the provider can be swapped without touching the analytics code.

---

## 3. Hypothesis testing and metrics in .NET

### Library landscape

| Option | Status | Fit |
|---|---|---|
| **MathNet.Numerics** | Stable 5.0.0 (Apr 2022). 6.0.0-beta2 (Mar 2025). Targets net48 / netstandard2.0 / net6 / net8, so it works on net10. About 47M downloads. | Has descriptive stats, distributions (Student-t, Normal, ChiSq), linear algebra and regression. **No packaged hypothesis-test API**, but t-tests and Jarque–Bera take a few lines on top of its distributions. Slow release cadence: a risk, not a blocker. |
| **Accord.NET** | **Archived 2020-11-19.** The author pointed to ML.NET as the successor. Community forks exist. | Avoid it. It had nice hypothesis-test classes, but it's unmaintained. |
| **ML.NET** | 5.0.1 (2026-10-15), with a 6.0 preview. | Built for ML pipelines (regression, anomaly detection, forecasting via SSA). It isn't a statistics-testing library. Useful later for anomaly or regime detection. |
| Skender.Stock.Indicators | Stable 2.7.x, with 3.0 previews. | Technical indicators (SMA, RSI, ATR…). Handy for "indicator X crosses Y" backtests. |
| Commercial (Numerics.NET / Extreme Optimization) | Full hypothesis-test suite. | Paid, so it **needs human approval**. Not needed for v1. |
| **Python sidecar** (scipy 1.18, statsmodels 0.15, pandas) | Mature and the industry standard. | Has the most complete test catalogue: OLS/HAC errors, ADF stationarity, Ljung-Box, bootstrap. It costs a second runtime and container, a second test stack and an extra network hop. |

### Tradeoffs

- **In-process .NET:** one deployable, typed, fast, easy to unit-test against golden values. The cost is that we hand-roll each test and must verify it.
- **Python sidecar:** gives correctness "for free" through vetted libraries and a richer econometrics set. The cost is ops overhead, versioning of a second service, and data marshalling.
- **Recommendation:** use .NET for the v1 metric set and add the sidecar when the first econometric story needs it. Verify every .NET implementation against scipy-generated **golden fixtures** checked into the repo, which gives us the sidecar's correctness without running it in production.

### v1 metric and test catalogue

**Metrics** (daily bars unless noted):

- Simple and log returns
- Cumulative return and CAGR
- Annualised volatility (√252)
- Sharpe (with a configurable risk-free rate)
- Sortino (downside deviation)
- Max drawdown, drawdown duration and Calmar ratio
- Beta / correlation vs. a benchmark (SPY)
- Rolling-window versions of the above
- Historical VaR / CVaR (95%)

**Hypothesis tests:**

- One-sample t-test: mean strategy return > 0
- Two-sample (Welch) t-test: strategy A vs. B, or vs. benchmark
- Paired t-test on aligned excess returns
- Jarque–Bera normality, as a warning that t-test assumptions may not hold
- *v1.1:* bootstrap confidence interval on Sharpe, and a Lo (2002)-style Sharpe standard error

**Backtest basics:**

- Vectorised daily backtest of simple signal rules (MA crossover, momentum, buy-and-hold)
- Close-to-next-open fills, fixed bps costs
- No look-ahead: signals at *t* trade at *t+1*
- Outputs the equity curve, metrics and the tests above
- Report sample size and, when several rules are compared, a multiple-testing warning (Bonferroni/Holm)

---

## 4. "AI" layer

### Sensible v1 shapes

1. **Narrative over computed metrics (v1).** The backend computes metrics and test results deterministically. The LLM gets a compact JSON fact sheet (metrics, p-values, sample sizes, date range, data source) and writes a plain-English summary.
   - Rules: it must quote only numbers present in the input, never invent figures, and always state limitations (sample size, normality, survivorship).
   - Validate the output by checking that every number in the text matches the fact sheet.
2. **Natural language → structured hypothesis (v1.1).** The user says, for example, "does AAPL outperform SPY on Mondays since 2020?" The LLM emits a **JSON hypothesis spec** (universe, date range, signal/filter, comparison, test, α), which is schema-validated and shown back to the user for confirmation, then run by the deterministic engine.
   - No LLM-generated code or SQL is executed.
3. **Later:** news or filings summarisation (that content has its own licensing), anomaly explanations, and a "what changed" daily digest.

**Implementation note:** `Microsoft.Extensions.AI` (10.11.0 on NuGet) gives a provider-neutral `IChatClient` abstraction in .NET, so the model vendor stays swappable. Any LLM API key spends money and is a secret, so it **needs human approval**.

### Risk and regulatory notes (US-centric, not legal advice)

- **Investment-adviser line.** Under *Lowe v. SEC* (1985), bona fide, impersonal publications of general and regular circulation fall within the publisher's exclusion from the Advisers Act. In 2024 a federal court applied this to Seeking Alpha, holding that subscriber filters and alerts didn't make the content personal.
  - Personalised recommendations ("you should buy X given your portfolio"), content timed to move prices, or touting securities we hold push us outside the exclusion.
  - **Design rule:** analysis only, no buy/sell/hold calls, no portfolio-specific advice, no managing user funds.
- **AI-washing.** On 2024-03-18 the SEC settled with Delphia ($225k) and Global Predictions ($175k) over false claims about their use of AI. Marketing copy must describe precisely what the AI does: "LLM-written summaries of statistically computed metrics," not "AI predicts the market."
- **Predictive-analytics rule.** The SEC's 2023 proposal on predictive data analytics conflicts was **withdrawn in June 2025** along with 13 other proposals. That removes a specific rule, not the general antifraud duties.
- **Disclaimers** on every analysis view and LLM output:
  - "for informational/educational purposes, not investment advice"
  - "past performance does not guarantee future results"
  - "statistical significance ≠ economic significance"
  - "data may be delayed", with the data source and timestamp
  - Have counsel review before anything goes public (**needs human approval**).
- **Data licensing applies to LLM output too.** Prices quoted in a generated narrative are still a display of vendor data. Derived figures (returns, Sharpe) count as "derived data" under some vendor terms, Massive's included. Sending vendor data to a third-party LLM API may itself be restricted, so check the vendor terms.
- **Hallucination controls:** numeric cross-checks, temperature 0, refusal templates for advice-seeking prompts ("should I buy…"), logging of prompt/response pairs, and a visible "AI-generated" label.

---

## 5. Hosting

### Option A: compose on a free-tier VM (Oracle Cloud Always Free)

- **Ampere A1 (Arm):** now **1,500 OCPU-hours / 9,000 GB-hours per month, about 2 OCPU / 12 GB**, cut in half from 4 / 24 in mid-2026. Oracle made no announcement, and it's unclear whether existing pay-as-you-go tenancies are grandfathered. Also included:
  - 2 × AMD micro VMs
  - 200 GB block storage
  - 10 TB/month egress
  - 2 × Autonomous DB (20 GB) or a single MySQL HeatWave free system
- **Idle reclamation:** Oracle may reclaim instances whose 95th-percentile CPU, network and memory all stay under 20% over 7 days. A low-traffic prototype could hit that, so plan a keep-alive or a scheduled job (the nightly data ingest helps), and keep backups.
- **Stack fit:** API (.NET 10, linux-arm64 images are available), Postgres 17 container, nginx serving the Vite build, optional Python sidecar, and Caddy/nginx for TLS. This fits easily in 2 OCPU / 12 GB.
- **Cost:** **$0/mo** within limits, plus a domain (about $10–15/yr, which needs approval).
- **Risks:** capacity errors on A1 in popular regions, the limit changes above, reclamation, and no managed backups. Mitigate with nightly `pg_dump` to Object Storage (free tier) and keeping everything in compose so the stack can move.

### Option B: AWS EC2 + RDS (as the README describes)

Prototype-scale, on-demand pricing in us-east-1 (rough estimate):

| Item | Approx. $/mo |
|---|---|
| EC2 t4g.small (2 vCPU / 2 GiB, Graviton) at $0.0168/hr | 12.3 |
| RDS PostgreSQL db.t4g.micro, single-AZ | ~12 |
| RDS storage 20 GB + EBS 20 GB gp3 | ~4 |
| Public IPv4 ($0.005/hr since 2024-02-01) | 3.7 |
| Data transfer, Route 53 zone, CloudWatch basics | ~1–3 |
| **Total** | **≈ $33–35/mo** (an ALB adds about $16+; avoid it at prototype scale) |

- **New accounts:** since July 2025, new accounts get up to **$200 in credits** and a free plan of up to 6 months, which would cover a prototype for a while. Opening an account and adding a card are human steps.
- **Savings:** a 1-year Savings Plan cuts EC2 about 28–37%.

### Recommendation

Use **Option A first (compose-first, as in star-with-me).** Keep the stack portable:

- one `docker-compose.yml`
- configuration through env vars
- standard Postgres, with no Oracle-specific DB features

When moving to AWS is justified (uptime SLA, managed backups, team access), the same images run on EC2 with compose. Swapping the Postgres container for RDS is then a connection-string change.

Optionally, run the same compose stack locally or in CI first, so nothing needs a cloud account until the human approves one.

---

## Proposed backlog

Legend:
- **[HA]** = needs human approval: spends money, bumps a major dependency, touches secrets, or deploys to a public URL.
- Stories are ordered, and each line gives its acceptance criterion (AC).

### Epic 0: Decisions (human)
- **0.1 [HA]** Decide the public vs. private scope for v1. *AC: written decision recorded in `docs/decisions/`, stating whether any page is publicly reachable before a data display licence exists.*
- **0.2 [HA]** Choose the hosting target (OCI Always Free vs. AWS) and who owns the account. *AC: ADR merged that names the provider, region and account owner.*
- **0.3 [HA]** Choose the market-data vendor for the prototype, plus a budget ceiling for production. *AC: ADR names the vendor(s), plan, monthly cap and licence class (personal/business).*

### Epic 1: Platform modernisation
- **1.1 [HA, major bump]** Retarget the API to `net10.0`. Update the Dockerfile to 10.0 images and fix its COPY paths. *AC: `dotnet build` and `docker build` succeed from a clean clone, and the container answers `GET /health` with 200.*
- **1.2** Wire up DI, built-in OpenAPI, health checks and structured logging. Remove the Products sample. *AC: `/openapi/v1.json` is served in Development, and no `new` of services happens in controllers.*
- **1.3 [HA, major bump]** Replace CRA with Vite + React 19 + TypeScript. Remove `react-scripts` and the placeholder `react-globe`. *AC: `npm ci && npm run build && npm test` pass on Node 22 LTS, and the dev server proxies `/api` to the backend.*
- **1.4** Decide on the globe: either `react-globe.gl` behind a lazy-loaded component, or drop it. *AC: the landing page renders without WebGL errors on Chrome and Firefox, or the globe has been removed and noted in an ADR.*
- **1.5** Add CI (GitHub Actions, build + test for both projects, no deploy). *AC: a PR shows green checks for the api and client jobs. No secrets used.*

### Epic 2: Local compose stack
- **2.1** `docker-compose.yml` with api, client (nginx), postgres and an optional `stats` profile. *AC: `docker compose up` on a clean machine serves the client at localhost and the API reaches Postgres.*
- **2.2** EF Core + Npgsql migrations for `instruments`, `bars_daily` and `analyses`. *AC: migrations apply on startup in Development, and an integration test round-trips a bar.*
- **2.3** Seed/fixture loader using bundled CSV sample data (synthetic or public-domain). *AC: the app is fully demoable with no vendor key.*

### Epic 3: Market data ingestion
- **3.1** Define `IMarketDataProvider` (daily bars, latest quote, stream). Add a fake provider for tests. *AC: analytics code depends only on the interface, and unit tests use the fake.*
- **3.2 [HA, secrets/account]** Alpaca adapter (REST daily bars + IEX WebSocket quotes), keys from env/secret store. *AC: with a key in the env, it backfills 5 years of SPY/AAPL daily bars. Without a key, it disables itself cleanly.*
- **3.3** Nightly incremental ingest job plus a rate-limit/backoff policy. *AC: a re-run is idempotent and logs counts, and a 429 triggers backoff rather than failure.*
- **3.4** Live quote fan-out via SignalR to the client (private use only). *AC: the client shows a live-updating last price for one symbol. The feature is flagged off in any public build.*

### Epic 4: Metrics and hypothesis engine (.NET)
- **4.1** Returns, volatility, Sharpe, Sortino, max drawdown/duration, Calmar, beta/correlation, VaR/CVaR on top of MathNet.Numerics. *AC: each metric matches scipy/numpy golden fixtures to 1e-9 on 3 fixture series.*
- **4.2** Hypothesis tests: one-sample, Welch and paired t-tests, Jarque–Bera. *AC: statistic and p-value match `scipy.stats` fixtures to 1e-6, and results include n, df and assumptions warnings.*
- **4.3** Vectorised daily backtester (signals at t, fills at t+1, bps costs) for buy-and-hold, MA-crossover and momentum. *AC: a look-ahead test proves that a signal using the t+1 close cannot trade, and the equity curve matches a hand-computed fixture.*
- **4.4** `POST /api/analyses` (instrument(s), range, metrics, tests, optional strategy) returns a JSON fact sheet. *AC: an OpenAPI-documented endpoint returns deterministic output for the fixture data.*
- **4.5** Multiple-comparison guard (Holm) when more than one strategy or test is requested. *AC: adjusted p-values are present and flagged in the response.*

### Epic 5: Client analytics UI
- **5.1** Instrument picker, date range, and metrics table and drawdown chart (`lightweight-charts`, with its attribution). *AC: running an analysis on fixture data renders the table and chart, and the Lighthouse accessibility score is at least 90.*
- **5.2** Hypothesis-test result view with plain-language labels and the disclaimer banner. *AC: every analysis view shows the source, data timestamp and "not investment advice" text.*

### Epic 6: AI narrative layer
- **6.1** `IChatClient` (Microsoft.Extensions.AI) abstraction with a stub/offline implementation. *AC: the narrative endpoint works end-to-end with the stub, and no key is required.*
- **6.2 [HA, spend + secrets]** Connect a real LLM provider, with a monthly budget cap and a key from the secret store. *AC: the narrative is generated for a fixture analysis and spend is logged per request.*
- **6.3** Grounding guard: every number in the narrative must appear in the fact sheet, otherwise regenerate or fall back to a template. Add advice-seeking refusal templates. *AC: a test suite of 20 adversarial prompts (e.g. "should I buy…") yields no recommendations and no unmatched numbers.*
- **6.4** NL → JSON hypothesis spec (schema-validated, user-confirmed before run). *AC: 15 of 20 sample questions parse to valid specs, and invalid specs are rejected with a reason. No code or SQL is executed from model output.*

### Epic 7: Optional Python stats sidecar
- **7.1** FastAPI service exposing scipy/statsmodels tests (OLS with HAC errors, ADF, Ljung-Box, bootstrap Sharpe CI) behind the compose `stats` profile. *AC: the API calls the sidecar over the internal network only, and contract tests pass in CI.*

### Epic 8: Hosting and release
- **8.1 [HA, account/spend]** Provision the OCI Always Free A1 VM (2 OCPU / 12 GB) in the chosen home region, with SSH keys and the firewall. *AC: the VM is reachable only via SSH and has docker + compose installed.*
- **8.2 [HA, secrets]** Secret management (env file on the VM with restricted permissions, or OCI Vault) and a rotation note. *AC: no secrets in the repo, verified by a secret-scan job in CI.*
- **8.3** Backups: nightly `pg_dump` to object storage, plus a restore drill. *AC: a documented restore into a fresh container succeeds.*
- **8.4** Activity keep-alive and monitoring to avoid idle reclamation. *AC: the VM's 7-day p95 CPU/network sits above the reclamation threshold, or the risk is documented and accepted.*
- **8.5 [HA, public URL]** Domain + TLS (Caddy) and a public deploy. *Blocked on 0.1 plus a display-licensed data plan, or synthetic-data-only mode.* *AC: HTTPS endpoint live, disclaimers present, legal review sign-off recorded.*
- **8.6 [HA, spend]** Production market-data licence (e.g. Twelve Data Venture, $149/mo) with display terms reviewed. *AC: a signed or accepted plan whose licence class covers external display is on file before any public page shows vendor data.*

### Epic 9: Compliance and content
- **9.1 [HA]** Disclaimer and terms-of-use copy, reviewed by a human or counsel. *AC: the approved text is stored in the repo and rendered on all analysis and AI views.*
- **9.2** Marketing/README language audit to remove AI-washing (describe the AI precisely). *AC: the README and landing copy describe the AI as "LLM summaries of computed statistics," and there are no predictive claims.*

---

## Sources

**.NET and tooling**
- Microsoft .NET support policy: https://dotnet.microsoft.com/en-us/platform/support/policy
- .NET 7 end of support (Azure update): https://azure.microsoft.com/updates/dotnet7support
- .NET release/support timeline, including the STS 24-month change and .NET 11 date: https://hidekazu-konishi.com/entry/dotnet_release_and_support_timeline.html
- .NET Conf 2026 / .NET 11 launch: https://devblogs.microsoft.com/dotnet/dotnet-conf-2026/
- CRA deprecation coverage (DevClass, 2025-02-18): https://devclass.com/2025/02/18/react-team-formally-deprecates-create-react-app-following-perfect-storm-of-incompatibility/
- Vite 7 announcement (Node requirements): https://vite.dev/blog/announcing-vite7
- react-globe repo and v3.0.0 transfer note: https://github.com/chrisrzhou/react-globe/releases/tag/v3.0.0
- react-globe usage stats: https://packages.ecosyste.ms/registries/npmjs.org/packages/react-globe
- react-globe.gl: https://github.com/vasturiano/react-globe.gl
- npm registry metadata (queried 2026-11-05 via `npm view` / `npm pack`) for react-globe, react-globe.gl, react-scripts, vite, react, three, lightweight-charts

**Statistics libraries**
- MathNet.Numerics on NuGet: https://www.nuget.org/packages/MathNet.Numerics/6.0.0-beta2
- Accord.NET archive notice: https://github.com/accord-net/framework/
- Microsoft.ML versions: https://nubrowse.com/packages/Microsoft.ML
- NuGet flat-container index (queried 2026-11-05) for MathNet.Numerics, Microsoft.ML, Microsoft.Extensions.AI, Skender.Stock.Indicators, Npgsql EF Core
- PyPI JSON API (queried 2026-11-05) for scipy and statsmodels

**Market data**
- Massive vs Finnhub (competitor page, checked 2026-10-23): https://massive.com/compare/massive-vs-finnhub.md
- Massive vs Tiingo: https://massive.com/compare/massive-vs-tiingo.md
- Massive vs Alpaca: https://massive.com/compare/massive-vs-alpaca.md
- Massive, "which plan to show data in my app": https://massive.com/knowledge-base/article/which-plan-do-i-need-to-show-massive-data-in-my-app
- Polygon → Massive rebrand pricing note (2026-09-12): https://apicostcalc.com/blog/polygon-massive-rebrand-api-pricing.html
- Alpaca Market Data API plans: https://docs.alpaca.markets/docs/about-market-data-api
- Alpaca redistribution FAQ: https://alpaca.markets/support/redistribute-alpaca-api
- Twelve Data pricing: https://twelvedata.com/pricing.md
- Finnhub listing: https://nexusfi.com/d/data-providers/finnhub/
- Alpha Vantage premium: https://www.alphavantage.co/premium
- Alpha Vantage alternatives (verified 2026-08-20): https://multiples.vc/compare/alpha-vantage-api-alternatives/md
- Tiingo IEX API: https://api.tiingo.com/products/iex-api
- Databento pricing: https://databento.com/pricing
- Databento usage pricing FAQ: https://databento.com/docs/faqs/usage-pricing-and-data-credits.md
- IEX Cloud shutdown / Blue Sky Data: https://www.integrity-research.com/iex-cloud-shuttered-though-former-execs-acquire-assets/

**Regulatory**
- Seeking Alpha publisher's-exclusion ruling (Greenberg Traurig, 2024-08): https://www.gtlaw.com/ar/insights/2024/8/no-need-for-seeking-alpha-to-seek-registration
- *Lowe v. SEC*, 472 U.S. 181 (1985): https://www.law.cornell.edu/supremecourt/text/472/181
- SEC AI-washing actions (WilmerHale, 2024-03): https://wilmerhale.com/insights/client-alerts/20240327-sec-brings-two-more-ai-washing-enforcement-actions-against-investment-advisers-continuing-its-pursuit-of-misstatements-related-to-ai
- SEC withdraws 14 proposals, including predictive data analytics (Ropes & Gray, 2025-06): https://www.ropesgray.com/en/insights/alerts/2025/06/sec-clears-unfinished-rulemakings-from-regulatory-agenda

**Hosting**
- Oracle Always Free resources: https://docs.oracle.com/en-us/iaas/Content/FreeTier/resourceref.htm
- Oracle A1 cut (InfoQ, 2026-07): https://infoq.com/news/2026/07/oracle-cloud-free-tier-limits/
- Oracle A1 cut (Linuxiac): https://linuxiac.com/oracle-quietly-cuts-free-tier-ampere-a1-resources-in-half/
- AWS Free Tier credits (2025-07): https://aws.amazon.com/about-aws/whats-new/2025/07/aws-free-tier-credits-month-free-plan/
- AWS public IPv4 charge: https://aws.amazon.com/blogs/aws/new-aws-public-ipv4-address-charge-public-ip-insights
- EC2 t4g.small pricing: https://calculator.holori.com/aws/ec2/t4g.small/us-east-1
- RDS db.t4g.micro pricing: https://www.bytebase.com/dbcost/rds/instance/db.t4g.micro.md
