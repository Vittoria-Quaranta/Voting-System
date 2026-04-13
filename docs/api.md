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
  "lastName": "Frazier"
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

## Running the backend

```bash
cd backend
dotnet run --project VotingSystem.View
```

It'll start on `http://localhost:5000`.
