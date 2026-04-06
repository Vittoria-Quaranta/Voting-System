# Voting-System
iDesign Feature List — Voting System (Pacopolis)
🧩 Client / Use Case Layer
These are the user-facing interactions the system must support:

Voter Login / Authentication — Voter identifies themselves securely before accessing the ballot.
Display Ballot — Present the voter with all current elections and issues after authentication.
Make Selections — Voter selects candidates/choices for each race and issue.
Review & Edit Selections — Voter can review their full ballot and change selections before submitting.
Submit Ballot — Final submission locks in the voter's choices.
View Recorded Vote — A voter can later look up their own vote to confirm it was recorded correctly.
Third-Party Voter Participation Check — A third party can confirm whether someone voted, but not how they voted.
Admin: Declare Winners — At end of voting day, system calculates and announces winners for each race/issue.


🏛 Manager Layer
Orchestrates business logic and workflows:

Election Manager — Manages the lifecycle of an election: setup, active voting period, and closure.
Ballot Manager — Assembles the correct ballot for each voter based on active elections.
Voter Session Manager — Tracks an in-progress voting session; handles review/edit flow before submission.
Duplicate Vote Prevention — Enforces the rule that a voter may cast a ballot only once per election.
Results Manager — Tallies votes and determines winners/outcomes at end of voting day.
Audit/Privacy Manager — Enforces privacy rules: separates voter identity from vote choices in storage and retrieval.


⚙️ Engine Layer
Core business logic and computation:

Vote Encryption/Anonymization Engine — Ensures the link between a voter's identity and their specific choices cannot be reconstructed by third parties.
Winner Determination Engine — Implements vote-counting rules for each election type (plurality, majority, yes/no issue, etc.).
Authentication Engine — Validates voter credentials and identity.
Ballot Validation Engine — Validates that all required selections are made before submission is allowed.


🗄 Resource Access Layer
Data persistence and external integrations:

Voter Database — Stores voter registration data, credentials, and voting status (voted / not voted).
Election & Ballot Database — Stores all elections, candidates, issues, and their configurations.
Vote Record Store — Stores submitted votes in a way that preserves anonymity (decoupled from voter identity).
Audit Log — Immutable log recording who voted (not how), timestamps, and system events.


🌐 Infrastructure / Cross-Cutting

Multi-Client Access Support — Allow voters to vote from personal computers or shared polling-location machines.
Role-Based Access Control — Separate access rules for voters vs. admins vs. third-party auditors.
Election Configuration Interface — Admin UI/tool to define elections, add candidates, set voting windows, and close elections.


This gives your team a clean separation of concerns to divide work across Scrum sprints. The two most critical items to tackle in Sprint 1 would be Authentication (#1/#17) and the core voting flow (#2–#6, #10–#12), leaving Results, Auditing, and Admin features for Sprint 2.

The 6 high-level features:
Voter Authentication — Voters securely identify themselves before accessing the ballot (login/credentials).
Ballot Management — Display the correct ballot to each voter, covering all active elections and issues.
Vote Casting & Review — Voter makes selections, reviews them, edits if needed, and submits their final ballot.
Duplicate Vote Prevention — Enforce that no voter can vote more than once in the same election.
Voter Privacy & Audit — Allow voters to confirm their own vote was recorded correctly, while making it impossible for third parties to see how anyone voted (only whether they voted).
Results & Winner Determination — At the end of voting day, tally all votes and determine the winner of each race and the outcome of each issue.

