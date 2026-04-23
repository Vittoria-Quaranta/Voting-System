# API Reference

Quick reference for the endpoints the frontend needs. Backend runs on `http://localhost:5000`. Vite proxies `/api` so you can just call `/api/login` etc from the frontend.

Test accounts are in `test-data.md`. All of them use the password `husker2026`.

## Login

**POST /api/login**

Send:
```json
{
  "username": "tfrazier",
  "password": "husker2026"
}
```

Get back (success):
```json
{
  "success": true,
  "message": "Login successful.",
  "voterId": 1,
  "firstName": "Tommie",
  "lastName": "Frazier",
  "hasVoted": false
}
```

Get back (bad login):
```json
{
  "success": false,
  "message": "Invalid username or password.",
  "voterId": null,
  "firstName": null,
  "lastName": null
}
```

The error message is the same for wrong username, wrong password, or empty input on purpose (so nobody can tell which field was wrong).

## Register

**POST /api/register**

Creates a new voter account. Password is hashed server-side (BCrypt) before storage. Always returns 200; check `success` in the body.

Send:
```json
{
  "firstName": "Test",
  "lastName": "User",
  "username": "testuser",
  "password": "husker2026",
  "dateOfBirth": "2000-01-01"
}
```

`dateOfBirth` can be `null` or omitted.

Get back (success):
```json
{
  "success": true,
  "message": "Registration successful.",
  "voterId": 42,
  "firstName": "Test",
  "lastName": "User"
}
```

Get back (failure):
```json
{
  "success": false,
  "message": "Username already taken.",
  "voterId": null,
  "firstName": null,
  "lastName": null
}
```

Possible error messages:
- First name is required.
- Last name is required.
- Username is required.
- Password is required.
- Password must be at least 8 characters.
- Username already taken.

## Get the ballot

**GET /api/ballot**

No body needed.

Response looks like this:
```json
{
  "electionId": 1,
  "electionName": "2026 Pacopolis General Election",
  "races": [
    {
      "raceId": 1,
      "raceName": "Mayor",
      "raceType": "Candidate",
      "choices": [
        { "choiceId": 1, "choiceName": "Lil Red", "party": "Spirit Party" },
        { "choiceId": 2, "choiceName": "Herbie Husker", "party": "Tradition Party" }
      ]
    }
  ]
}
```

Some notes:
- `raceType` is either `"Candidate"` (regular race) or `"YesNo"` (proposition with Yes/No choices)
- For Yes/No, the choices are named "Yes" and "No" but still use the `choiceName` field
- `party` can be null (for nonpartisan races or yes/no stuff)
- Returns 404 if there's no active election

## Submit the ballot

**POST /api/submit-ballot**

Send:
```json
{
  "voterId": 1,
  "electionId": 1,
  "selections": [
    { "raceId": 1, "choiceId": 1 },
    { "raceId": 2, "choiceId": 3 },
    { "raceId": 3, "choiceId": 6 }
  ]
}
```

You need one selection for each race on the ballot. The backend will reject it if anything is missing.

Get back (success):
```json
{
  "success": true,
  "message": "Ballot submitted successfully.",
  "confirmationCode": "055b02cd-fd05-4b79-b341-11be0e6ae426"
}
```

Show the `confirmationCode` on the confirmation page so the voter can save it.

Get back (failure):
```json
{
  "success": false,
  "message": "You have already voted in this election.",
  "confirmationCode": null
}
```

Possible error messages:
- No active election to submit to
- You have already voted in this election
- No selections were submitted
- Duplicate selection for a race
- Missing selection for race: {name}
- Selection for unknown race: {id}
- Selection for unknown choice in race: {name}

## Get election results

**GET /api/results**

No body needed. Returns vote counts and winners for the active election.

```json
{
  "electionId": 1,
  "electionName": "2026 Pacopolis General Election",
  "races": [
    {
      "raceId": 1,
      "raceName": "Mayor",
      "raceType": "Candidate",
      "totalVotes": 175,
      "candidates": [
        {
          "candidateId": 1,
          "candidateName": "Lil Red",
          "party": "Spirit Party",
          "votes": 100,
          "percentage": 57.1,
          "isWinner": true
        },
        {
          "candidateId": 2,
          "candidateName": "Herbie Husker",
          "party": "Tradition Party",
          "votes": 75,
          "percentage": 42.9,
          "isWinner": false
        }
      ]
    }
  ]
}
```

- Candidates sorted by votes descending
- Ties mark both as winner
- Returns 404 if no active election

## Look up a recorded vote

**GET /api/vote-lookup/{confirmationCode}**

The voter enters the confirmation code they saved after submitting. Returns the selections the ballot recorded so the voter can verify nothing was changed.

Response (200):
```json
{
  "electionId": 1,
  "electionName": "2026 Pacopolis General Election",
  "selections": [
    {
      "raceId": 1,
      "raceName": "Mayor",
      "raceType": "Candidate",
      "candidateId": 1,
      "candidateName": "Lil Red",
      "party": "Spirit Party"
    },
    {
      "raceId": 2,
      "raceName": "Proposition A",
      "raceType": "YesNo",
      "candidateId": 5,
      "candidateName": "Yes",
      "party": null
    }
  ]
}
```

Returns 404 if the confirmation code is not found. The code itself is the only authorization, so anyone holding it can read the ballot, that is acceptable for the demo because the code is only ever shown to the voter.

## Check voter participation

**GET /api/participation?username={username}**

Third parties can check whether a voter has participated in the active election. Never reveals choices, only yes or no.

Response (200):
```json
{ "voted": true }
```

```json
{ "voted": false }
```

Unknown usernames return `{"voted": false}` (same shape as "has not voted") so callers cannot enumerate which usernames exist. Returns 400 if the `username` query parameter is missing or empty.

## Dev-only endpoints

These only exist when running in Development mode.

**POST /api/dev/reset-votes** - clears all votes and voter records from the database.

## Running the backend

```bash
cd backend
ASPNETCORE_ENVIRONMENT=Development dotnet run --project VotingSystem.View
```

It'll start on `http://localhost:5000`. The `Development` flag is required to load the real database connection string.
