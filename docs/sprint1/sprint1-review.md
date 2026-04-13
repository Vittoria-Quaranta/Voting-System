# Sprint 1 Review

**Team:** Pacopolis Voting System
**Sprint:** 1
**Date:** 2026-04-13

## Goal

Get the core voting flow working. Log in, see a ballot, submit it, get a confirmation. Backend ready, frontend at least started.

## What got done

### Backend (done)
- Database schema on Azure SQL. Six tables: Voter, Election, Race, Candidate, VoterRecord, Vote
- Seed data with Husker stuff so we had something real to test with
- Three accessors: voter, election, vote. Dapper for SQL, interfaces so we can mock them
- Auth engine with BCrypt for password hashing
- Auth manager for the login flow, same error message on fail so you can't guess what was wrong
- Ballot manager that loads the active election with all races and choices
- Ballot validation engine, catches missing picks, dupes, bad race/choice IDs
- Duplicate vote engine, separate from the DB check so it fails cleanly
- Voting session manager that runs the whole submit: load, check dupe, validate, save
- Three API routes: POST /api/login, GET /api/ballot, POST /api/submit-ballot
- 31 unit tests, all green

### Docs
- README with setup
- API reference doc
- Test account list

### Frontend (not done)
- React scaffolded
- Draft PRs open for roughing out pages and wiring them to the API
- Nothing merged yet

## What didn't get done

- All frontend stuff (rough-outs and backend wiring)
- UI screenshots since the pages aren't built
- Results page (pushed to Sprint 2 anyway)

## Architecture

We're using iDesign. Layers:
- **View** = ASP.NET Core minimal API (the routes)
- **Managers** = orchestration
- **Engines** = pure logic, no DB
- **ResourceAccess** = all the SQL stuff
- **DataContracts** = DTOs and entities everyone shares

Dependencies only go one way: View -> Managers -> Engines/Accessors -> DataContracts. Everything behind an interface so tests can mock.

Rough architect drawing needs to be made digital.

SOLID stuff:
- One job per class
- Depends on interfaces, not concrete classes
- Tests use hand-rolled fakes

## Screenshots

- [x] GitHub project board - [view](assets/github-projects-board.png)
- [x] VS Code file tree showing the solution - [view](assets/solution-explorer.png)
- [x] Azure SQL tables with some data - [view](assets/sql-tables.png)
- [x] `dotnet test` output, all 31 passing - [view](assets/dotnet-tests.png)
- [ ] iDesign architecture diagram

## Review meeting notes

**Agenda (what to cover):**
- Walk through what got built this sprint using the screenshots above
- Walk through the iDesign layers
- Show tests passing (screenshot above)
- Talk about the frontend plan and Sprint 2

**Notes (fill in during/after):**

- 
- 
- 
- Instructor feedback:
