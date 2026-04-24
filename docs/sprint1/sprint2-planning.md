# Sprint 2 Planning

**Team:** Pacopolis Voting System
**Sprint:** 2
**Date:** 2026-04-13

## Goal

Finish the frontend, get the results/reporting feature working, and round out the system with the stuff that didn't fit in Sprint 1. By end of sprint we want a full demo where someone can log in, vote, see a confirmation, and view results.

## Carrying over from Sprint 1

Frontend stuff didn't get finished. Two draft PRs already exist for them:

**`feature/frontend-rough-out`** (rough page stubs)
- #6 Rough out login page
- #7 Rough out ballot page
- #8 Rough out review page
- #9 Rough out confirmation page
- #10 Rough out results page

**`feature/frontend-wiring`** (wire pages to backend, blocked by rough-out)
- #11 Get login page working
- #22 Build the ballot page
- #25 Build the review page
- #32 Build confirmation page

These need to merge first so we have something to demo and look pretty.

## New Sprint 2 work

### Results / reporting feature
- #38 Decide what counts as our report feature
- #39 Add vote counting logic (engine)
- #40 Add results API route
- #41 Build the results page (frontend)
- #42 Write results tests

### Voter lookup and participation check
- #43 Let voters check that their vote was recorded (uses confirmation code)
- #44 Add voter participation check (third party can see if someone voted but not how)

### Voter registration (new feature)
- #63 Voter registration (so we can add new users without manual SQL)

### Admin / dev tools
- #64 Dev/demo log page (shows confirmation codes, votes, useful for demos)
- #65 Admin/election management (create elections, add races/candidates, open/close)

### Quality / process
- #45 Separate voter identity from vote data better (privacy hardening)
- #46 Set up static analysis
- #47 Add accessor tests
- #48 Expand tests for Sprint 2 features
- **NEW: GitHub Actions CI** - build + test on every PR, block merge if tests fail
- **NEW: Code coverage reporting** - coverlet + ReportGenerator to prove 80% on managers/engines
- **NEW: Branch protection on dev** - require PRs, require passing checks

## Plan

Rough split:

**Frontend**
- Finish rough-out and wiring (Sprint 1 carryover)
- Build the results page once the API route is up
- Build the dev/demo log page
- Build the voter registration form
- Build the admin/election management UI

**Backend**
- Vote counting logic + results API
- Voter registration backend
- Voter participation check route
- Recorded vote lookup route
- Static analysis setup
- More tests
- CI/CD pipeline with GitHub Actions
- Code coverage reporting

The split isn't strict, anyone can grab anything that's unblocked.

## Risks

- **Time.** Sprint 1 frontend slipped, we should give it priority early so it doesn't slip again.
- **Azure SQL auto-pause.** Free tier serverless pauses after inactivity, sometimes takes a minute to wake up. Annoying during testing.
- **Static analysis setup might surface a bunch of warnings** that take time to clean up. Worth budgeting for.

## Definition of done

A feature is done when:
- Code is merged into dev
- Has tests where it makes sense
- Issue is closed
- Works in a manual demo
