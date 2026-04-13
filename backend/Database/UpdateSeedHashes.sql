-- One-time migration to update existing voter rows with real BCrypt hashes
-- Password for all test voters: husker2026

UPDATE Voter
SET PasswordHash = '$2a$11$1oaH10kUk5v9U8NqqCySDu2pY22jx9goAZPCQ6m/Zuat09w.WTDP2'
WHERE PasswordHash = 'not_hashed_yet';
