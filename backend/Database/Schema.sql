-- Pacopolis Voting System - Database Schema
-- Run this against LocalDB instance to set up the tables

-- stores registered voters. passwords get hashed in C# before saving here(not implemented yet)
CREATE TABLE Voter (
    VoterId          INT              IDENTITY(1,1) PRIMARY KEY,
    FirstName        NVARCHAR(100)    NOT NULL,
    LastName         NVARCHAR(100)    NOT NULL,
    Username         NVARCHAR(50)     NOT NULL UNIQUE,
    PasswordHash     NVARCHAR(256)    NOT NULL,
    DateOfBirth      DATE             NULL,
    RegistrationDate DATETIME2        NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Election (
    ElectionId       INT              IDENTITY(1,1) PRIMARY KEY,
    ElectionName     NVARCHAR(200)    NOT NULL,
    StartDate        DATE             NOT NULL,
    EndDate          DATE             NOT NULL,
    IsActive         BIT              NOT NULL DEFAULT 0  -- flip to 1 when election is open for voting
);

-- each election has multiple races (example: mayor, city council, something else)
CREATE TABLE Race (
    RaceId           INT              IDENTITY(1,1) PRIMARY KEY,
    ElectionId       INT              NOT NULL
        REFERENCES Election(ElectionId),
    RaceName         NVARCHAR(200)    NOT NULL,
    RaceType         NVARCHAR(50)     NOT NULL DEFAULT 'Candidate'  -- 'Candidate' or 'YesNo' for ballot issues
);

CREATE TABLE Candidate (
    CandidateId      INT              IDENTITY(1,1) PRIMARY KEY,
    RaceId           INT              NOT NULL
        REFERENCES Race(RaceId),
    CandidateName    NVARCHAR(200)    NOT NULL,
    Party            NVARCHAR(100)    NULL  -- null for nonpartisan races or yes/no issues
);

-- tracks WHO voted, but not what they picked
-- this is the table third parties can query to check if someone participated
-- the unique constraint is how we prevent someone from voting twice in the same election
CREATE TABLE VoterRecord (
    VoterRecordId    INT              IDENTITY(1,1) PRIMARY KEY,
    VoterId          INT              NOT NULL
        REFERENCES Voter(VoterId),
    ElectionId       INT              NOT NULL
        REFERENCES Election(ElectionId),
    ConfirmationCode UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    SubmittedAt      DATETIME2        NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_OneVotePerElection UNIQUE (VoterId, ElectionId)
);

-- actual ballot selections go here
-- NOTE: no VoterId column on purpose. the only link back to the voter is through
-- ConfirmationCode, which only the voter knows. this keeps votes private even
-- if someone has db access — you can't go from voter -> their picks without the code
CREATE TABLE Vote (
    VoteId           INT              IDENTITY(1,1) PRIMARY KEY,
    ConfirmationCode UNIQUEIDENTIFIER NOT NULL,
    RaceId           INT              NOT NULL
        REFERENCES Race(RaceId),
    CandidateId      INT              NOT NULL
        REFERENCES Candidate(CandidateId),

    CONSTRAINT UQ_OneChoicePerRace UNIQUE (ConfirmationCode, RaceId)
);
