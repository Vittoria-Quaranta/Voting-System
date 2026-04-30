
# Sprint 2 Review

**Team:** Pacopolis Voting System
**Sprint:** 2
**Date:** 2026-04-24

## Goal

Finish frontend, add results/reporting, voter lookup, participation check. Full voting demo end-to-end.

## What got done

### CI/CD
- GitHub Actions: build + test on every PR
- Code coverage with 80% threshold on managers/engines
- Branch protection on dev

### Frontend (Sprint 1 carryover, completed)
- Tailwind CSS + reusable UI components
- Shared voting state across pages (VotingContext)
- Login, Ballot, Review, Confirmation pages wired to backend
- Already-voted detection on login
- UNL color scheme

### Results / Reporting
- Vote counting engine with winner determination
- Results API route and frontend page
- Results engine tests

### Dev Tools
- Dev-only tools page: reset votes, quick login, nav shortcuts
- Dev-only reset endpoint

### Voter Lookup + Participation
- TODO

### Docs
- README updated with run instructions and test accounts
- API reference updated

## What didn't get done

- TODO (fill in at end of sprint)

## Architecture

Same iDesign layers as Sprint 1, added:
- **ResultsEngine** - winner determination, percentages
- **ResultsManager** - orchestrates results
- **GET /api/results** - new route
- **VotingContext** - shared frontend state

Total tests: 37 (was 31 in Sprint 1)

## Screenshots

- [ ] GitHub project board
- [x] UI: Login page
- [x] UI: Ballot page with selections
- [X] UI: Review page
- [x] UI: Confirmation page
- [ ] UI: Results page
- [ ] Test output, all passing
- [ ] CI workflow passing
- [ ] Code coverage report

     ![image](https://github.com/user-attachments/assets/ec1d2379-a594-452e-85fa-8009b5893200)
  ![image](https://github.com/user-attachments/assets/8bc2cc43-fb23-43e8-9ad2-99276affa8d0)
![verifying vote](https://github.com/user-attachments/assets/e6e2ad3e-23f4-4cf0-a55e-483fa04cefcc)
![participation check](https://github.com/user-attachments/assets/894bf1d5-09b9-43b9-9086-a0759231f80c)
![Architecture explorer](https://github.com/user-attachments/assets/96ee5004-e964-46e7-9aef-acad4ae342cc)





## Review meeting notes

**Agenda:**
- Demo full voting flow
- Show already-voted detection
- Show dev tools
- Walk through results engine and tests
- Show CI and coverage
- Discuss what's left before final presentation

**Notes (fill in during/after):**
- 
