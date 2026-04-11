# Voting System
Pacopolis electronic voting system for CSCE 361.

## Client / Use Case Layer
These are the user-facing interactions the system must support:

- Voter Login / Authentication — Voter identifies themselves securely before accessing the ballot.
- Display Ballot — Present the voter with all current elections and issues after authentication.
- Make Selections — Voter selects candidates/choices for each race and issue.
- Review & Edit Selections — Voter can review their full ballot and change selections before submitting.
- Submit Ballot — Final submission locks in the voter's choices.
- View Recorded Vote — A voter can later look up their own vote to confirm it was recorded correctly.
- Third-Party Voter Participation Check — A third party can confirm whether someone voted, but not how they voted.
- Declare Winners — At end of voting day, system calculates and announces winners for each race/issue.

## Manager Layer
Orchestrates business logic and workflows:

- Election Manager — Manages the lifecycle of an election: setup, active voting period, and closure.
- Ballot Manager — Assembles the correct ballot for each voter based on active elections.
- Voter Session Manager — Tracks an in-progress voting session; handles review/edit flow before submission.
- Duplicate Vote Prevention — Enforces the rule that a voter may cast a ballot only once per election.
- Results Manager — Tallies votes and determines winners/outcomes at end of voting day.
- Audit/Privacy Manager — Enforces privacy rules: separates voter identity from vote choices in storage and retrieval.

## Engine Layer
Core business logic and computation:

- Vote Encryption/Anonymization Engine — Helps prevent a third party from linking a voter to their choices.
- Winner Determination Engine — Implements vote-counting rules for each election type (plurality, majority, yes/no issue, etc.).
- Authentication Engine — Validates voter credentials and identity.
- Ballot Validation Engine — Validates that all required selections are made before submission is allowed.

## Resource Access Layer
Data persistence and external integrations:

- Voter Database — Stores voter registration data, credentials, and voting status (voted / not voted).
- Election & Ballot Database — Stores all elections, candidates, issues, and their configurations.
- Vote Record Store — Stores submitted votes in a way that preserves anonymity (decoupled from voter identity).
- Audit Log — Immutable log recording who voted (not how), timestamps, and system events.

## Infrastructure / Cross-Cutting

- Multi-Client Access Support — Allow voters to vote from personal computers or shared polling-location machines.
- Role-Based Access Control — Separate access rules for voters vs. admins vs. third-party auditors.
- Election Configuration Interface — Admin UI/tool to define elections, add candidates, set voting windows, and close elections.

---

## Setup

**Requirements:** .NET 8 SDK, Node.js 18+, SQL Server (Developer or Express)

**Repo layout:**
- `frontend/` — React app (Vite)
- `backend/` — C# solution (View, Managers, Engines, ResourceAccess, DataContracts, Tests)

**Set up the database:**
1. Open SQL Server Object Explorer in Visual Studio
2. Connect to `(localdb)\MSSQLLocalDB`
3. Create a new database called `PacopolisVoting`
4. Open and run `backend/Database/Schema.sql` against it

**Run the backend:**
```bash
cd backend
dotnet restore
dotnet build
dotnet run --project VotingSystem.View
```

**Run the frontend:**
```bash
cd frontend
npm install
npm run dev
```

**Run tests:**
```bash
cd backend
dotnet test
```
