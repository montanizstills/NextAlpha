# Agentic working agreement (v2 draft)

How the human and Claude agent sessions work together on any project. v1 lives on Confluence ("Agentic working agreement", STARWITHME space). This draft adds branching and session rules. It is meant to move to its own repo so every project can adopt it (NXT-13).

> **Status: draft. Breaking change for every agent session.** It needs acknowledgement from the human and the star-with-me Lead before it replaces v1.

## 1. Coordination

- Each project has one **Lead** Claude session. It is the master: other sessions (workers, QA, research) send proposals and questions to it. The project's `AGENTS.md` names the Lead.
- **Work happens out of Jira.** Every piece of work has an issue in the project's Jira space.
  - Before starting or transitioning an issue, read its comments. The human's comments are instructions.
  - Answer in the same thread.
  - Before Done, post a proof comment listing each acceptance criterion with its evidence (commands run, output tails, CI links). Redact secrets.
- Items that need the human are labelled `human-review`.

## 2. Branches and environments (new)

- **Env branches** are `dev`, `qa`, `staging` and `prod`. **No one ever writes directly to an env branch.** That means no commits, pushes, force-pushes or rebases on them, by agents or by the human's own tooling.
- **All work happens on short-lived topic branches** cut from `dev`:
  - `feat/<KEY>-<slug>` for features (e.g. `feat/NXT-19-dotnet10`)
  - `fix/<KEY>-<slug>`, `chore/<KEY>-<slug>`, `docs/<KEY>-<slug>`
  - Use the Jira key when one exists.
- **PRs** go from a topic branch into `dev`. They are opened as drafts and marked ready when CI is green and the proof comment is posted. The PR title starts with the Jira key.
- **Promotion** moves `dev → qa → staging → prod`, only by PR, and only with human approval.
- **Stacked work:** a branch may build on another open topic branch. Say so in the PR body, and retarget it to `dev` once the base merges.
- **Before pushing:** fetch the base and rebase your own topic branch if needed. Never rebase or force-push a branch someone else is working on.
- **One worktree per agent.** Each worker gets its own git worktree, e.g. `<repo>/.worktrees/<name>` excluded via `.git/info/exclude`, so agents never share a checkout.

## 3. Needs explicit human approval

The human gives it as a comment on the issue, or in the Lead chat.

- Merging any PR. Promoting between env branches. Releasing a Jira version.
- Spending money: purchases, plan or quota upgrades, paid API calls, live payments.
- Secrets: creating, rotating or reading out a value. Changing GitHub secrets, variables, **branch protection, rulesets, default branch** or required checks.
- Deleting: branches, volumes, hosted data, Jira issues, Confluence pages, workspaces.
- Hosted environments: deploys to a public URL, DNS, firewall and ports, migrations on hosted data.
- Bumping pinned or **major** dependency versions (frameworks, SDKs, build toolchains).
- Anything public or outward-facing: publishing, emailing real people, changing repo visibility, **creating repos**.
- Any issue labelled `human-review`.

## 4. Breaking-change heads-up

Before starting a task that changes something other people or agents depend on, follow the steps below. Examples of such dependencies: an API contract, env var names, compose service names or ports, data schemas, a CI workflow, a version's scope, or **this agreement**.

1. Post a comment starting **"Breaking change heads-up"** on the issue, saying what changes and who is affected.
2. Add the label `breaking-change`.
3. Wait for the human's acknowledgement before merging. Work on the branch can continue in the meantime.

## 5. Autonomous by default

Without asking, agents may:

- create topic branches and worktrees, commit, and push topic branches;
- open and update draft PRs;
- run builds and tests, and add CI jobs that use no secrets;
- comment on issues, create subtasks, and move their own issues with proof comments;
- research using public sources only. No signing up for services, no creating keys.

## 6. Sessions (new)

- Workers report to the Lead. The Lead relays decisions to the human, and the human's approvals back to the workers.
- A session that can't do something because of its environment says so. Examples: a cloud session can't reach admin APIs or local machines. It files the step as a `human-review` task with exact commands, rather than working around the block.
- Handoffs between sessions go through a committed `HANDOFF.md` (or the Jira epic). Never rely on chat history alone.

## 7. Adopting this in a project

Add an `AGENTS.md` at the repo root containing:

- a link to this agreement (pinned to a version);
- the Jira project key and the epic;
- the Lead session and its worker roster;
- the env branches, the CI checks, and any project-specific approval rules.
