# Voting System Architecture Design

This project uses a layered architecture that aligns with MVC-style separation of concerns:

- `VotingSystem.View` is the presentation/composition layer. In the current implementation it is an ASP.NET minimal API entry point rather than MVC controllers.
- `VotingSystem.Managers` acts as the application/controller layer. Managers orchestrate use cases and depend on abstractions.
- `VotingSystem.Engines` acts as the domain/business-logic layer. Engines contain pure logic and no database access.
- `VotingSystem.ResourceAccess` acts as the data/model access layer. Accessors handle persistence through Dapper and SQL Server.
- `VotingSystem.DataContracts` contains entities and DTOs shared between layers.

## Database Design

The database schema is defined in [Schema.sql](/Users/cam/Voting-System/backend/Database/Schema.sql). The backend persists data in six core tables:

### Schema Summary

| Table | Purpose | Primary Key |
| --- | --- | --- |
| `Voter` | Stores registered voter accounts and hashed credentials. | `VoterId` |
| `Election` | Stores each election event. | `ElectionId` |
| `Race` | Stores contests/issues belonging to an election. | `RaceId` |
| `Candidate` | Stores candidates or yes/no options for a race. | `CandidateId` |
| `VoterRecord` | Stores proof that a voter participated in an election. | `VoterRecordId` |
| `Vote` | Stores the actual ballot selections made under a confirmation code. | `VoteId` |

### Primary and Foreign Key Relationships

| Parent Table | Child Table | Relationship | Key |
| --- | --- | --- | --- |
| `Election` | `Race` | One election has many races. | `Race.ElectionId -> Election.ElectionId` |
| `Race` | `Candidate` | One race has many candidates/options. | `Candidate.RaceId -> Race.RaceId` |
| `Voter` | `VoterRecord` | One voter can have voter records across elections. | `VoterRecord.VoterId -> Voter.VoterId` |
| `Election` | `VoterRecord` | One election has many participation records. | `VoterRecord.ElectionId -> Election.ElectionId` |
| `Race` | `Vote` | One race can receive many votes. | `Vote.RaceId -> Race.RaceId` |
| `Candidate` | `Vote` | One candidate can receive many votes. | `Vote.CandidateId -> Candidate.CandidateId` |

### Entity Relationship Narrative

- `Election` is the top-level voting event.
- Each `Election` contains one or more `Race` rows.
- Each `Race` contains one or more `Candidate` rows. For proposition-style races, the rows can represent `Yes` and `No`.
- `VoterRecord` links a `Voter` to an `Election` and proves that the voter participated.
- `Vote` stores the selections made for each race.
- `Vote` intentionally does not store `VoterId`; it stores `ConfirmationCode` so ballot choices are separated from voter identity.

### Integrity Constraints

- `Voter.Username` is `UNIQUE`, so two accounts cannot share the same username.
- `VoterRecord` has `UNIQUE (VoterId, ElectionId)`, which enforces one vote per voter per election.
- `Vote` has `UNIQUE (ConfirmationCode, RaceId)`, which enforces one recorded choice per race on a ballot.
- `VoterRecord.ConfirmationCode` is generated with `NEWID()` and is used by the vote lookup feature.

### Privacy-Oriented Design Detail

The schema deliberately separates participation from ballot content:

- `VoterRecord` answers "did this voter vote in this election?"
- `Vote` answers "what selections were recorded for this confirmation code?"

This means administrators can confirm participation through `VoterRecord` without directly storing a `VoterId` on each `Vote` row.