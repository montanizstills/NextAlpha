# AGENTS.md: NextAlpha

Agents working in this repo follow the **agentic working agreement**: `docs/agreement/WORKING-AGREEMENT-v2.md`. That is the draft v2, and it moves to its own repo under NXT-13. v1 is on Confluence (STARWITHME → "Agentic working agreement").

| | |
|---|---|
| Jira | project `NXT` on star-with-me.atlassian.net, epic **NXT-1** "NextAlpha MVP: milestones" |
| Lead | the NextAlpha Lead Claude session (see `HANDOFF.md` for current state) |
| Env branches | `dev` → `qa` → `staging` → `prod`. **Never write to these directly.** |
| Topic branches | `feat/NXT-<n>-<slug>`, `fix/…`, `chore/…`, `docs/…`, cut from `dev`; PR into `dev` |
| Worktrees | one per agent under `.worktrees/<name>` (excluded via `.git/info/exclude`) |
| CI | `.github/workflows/backend-ci.yml` (build + test). A client job is tracked in NXT-17 |

## Layout
- `nextalpha-application/`: ASP.NET API (moving to .NET 10 under NXT-19), with `NextAlpha.sln` at the root
- `nextalpha-application.Tests/`: xUnit plus WebApplicationFactory tests
- `nextalpha-client/`: React client (toolchain decision in NXT-11)
- `docs/research/`: R&D brief, on branch `agent/research` until merged

## Project-specific approval rules
- Any page reachable from the public internet that shows market data, or numbers derived from it, needs NXT-8 decided and a display-licensed data plan.
- AI output must stay impersonal analysis of computed statistics, never investment advice.
