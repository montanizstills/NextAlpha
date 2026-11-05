# HANDOFF: NextAlpha Lead session

Written 2026-11-05 by the cloud Lead session (claude.ai chat). It is meant for a **Claude Code session on the Mac Studio** taking over as NextAlpha Lead. Read `AGENTS.md` and `docs/agreement/WORKING-AGREEMENT-v2.md` first.

## Start here
1. `gh auth status` must be logged in. The cloud session's GitHub admin API was blocked, so the steps below need this Mac.
2. Read Jira epic **NXT-1** and its children. The human's comments there are instructions.
3. Coordinate with the star-with-me Lead session on NXT-13 (working agreement v2).

## State of the repo (github.com/montanizstills/NextAlpha)

| Branch | What | PR |
|---|---|---|
| `main` | 2023 README only. To be renamed `prod` (NXT-12) | |
| `dev`, `qa`, `staging` | created at `af0623b` (migrated code) | |
| `feat/bootstrap-prod` | merges main's unrelated history with the code | #3 → main, draft |
| `agent/backend` | scaffold fixes, DI, health, Swagger, 21 xUnit tests, backend CI (green) | #1 → dev, draft, NXT-14/15 |
| `agent/client` | client builds/tests on Node 22, API helper, /health, dev proxy | #2 → dev, draft, NXT-16 |
| `feat/dotnet10-upgrade` | .NET 10 (SDK 10.0.403, built-in OpenAPI, port 80 kept via `ASPNETCORE_HTTP_PORTS`), 21/21 tests, CI green; stacked on #1 | #4 → dev, draft, NXT-19 |
| `agent/research` | R&D brief `docs/research/2026-11-rnd-brief.md` | no PR yet |
| `feat/agents-md` | this file, AGENTS.md, agreement v2 draft | → dev |
| `master`, `project-migration` | superseded. Deleting them needs approval | |

## Pending on the Mac (NXT-12; each item needs the human's OK as noted)
- [ ] `gh api -X POST repos/montanizstills/NextAlpha/branches/main/rename -f new_name=prod` (the human asked for this)
- [ ] Rename `agent/backend` → `feat/NXT-14-backend-scaffold`, `agent/client` → `feat/NXT-16-client-scaffold` and `agent/research` → `docs/rnd-brief`. The rename API keeps the PRs attached.
- [ ] Default branch → `dev` (confirm with the human)
- [ ] Protect `dev`/`qa`/`staging`/`prod`: PR required, no direct push, CI required on `dev` (**explicit approval**)
- [ ] Create the shared agreement repo, after the human approves its name and visibility (NXT-13)

## Decisions waiting on the human (label `human-review`)
- NXT-8: public or private v1
- NXT-9: data vendor and budget
- NXT-10: hosting (Oracle Always Free vs AWS)
- NXT-11: react-scripts 5 vs Vite + React 19, and react-globe
- Merges of PRs #1, #2, #3, #4
- PR #4 notes: the OpenAPI document is now 3.1.1 rather than 3.0.x (nothing in the repo consumes it yet). Moving the container port to 8080 is deferred.

## Approved already
- .NET 10 upgrade (human, 2026-11-05) → NXT-19
