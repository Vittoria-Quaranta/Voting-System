-- Seed data for testing

-- Voters
INSERT INTO Voter (FirstName, LastName, Username, PasswordHash)
VALUES
    ('Tommie', 'Frazier', 'tfrazier', 'not_hashed_yet'),
    ('Eric', 'Crouch', 'ecrouch', 'not_hashed_yet'),
    ('Brook', 'Berringer', 'bberringer', 'not_hashed_yet'),
    ('Johnny', 'Rodgers', 'jrodgers', 'not_hashed_yet'),
    ('Mike', 'Rozier', 'mrozier', 'not_hashed_yet');

-- Election
INSERT INTO Election (ElectionName, StartDate, EndDate, IsActive)
VALUES ('2026 Pacopolis General Election', '2026-04-01', '2026-04-30', 1);

-- Races
INSERT INTO Race (ElectionId, RaceName, RaceType)
VALUES
    (1, 'Mayor', 'Candidate'),
    (1, 'City Council', 'Candidate'),
    (1, 'Proposition 1: Should Pacopolis build a new Memorial Stadium?', 'YesNo');

-- Mayor candidates
INSERT INTO Candidate (RaceId, CandidateName, Party)
VALUES
    (1, 'Lil Red', 'Spirit Party'),
    (1, 'Herbie Husker', 'Tradition Party');

-- City Council candidates
INSERT INTO Candidate (RaceId, CandidateName, Party)
VALUES
    (2, 'Scott Frost', NULL),
    (2, 'Tom Osborne', NULL),
    (2, 'Frank Solich', NULL);

-- Proposition 1
INSERT INTO Candidate (RaceId, CandidateName, Party)
VALUES
    (3, 'Yes', NULL),
    (3, 'No', NULL);
