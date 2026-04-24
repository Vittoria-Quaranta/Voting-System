using Dapper;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("VotingDb")
    ?? throw new InvalidOperationException(
        "Missing 'VotingDb' connection string. " +
        "Copy appsettings.Development.json.template to appsettings.Development.json and fill in your credentials.");

// accessors (data layer)
builder.Services.AddScoped<IVoterAccessor>(_ => new VoterAccessor(connectionString));
builder.Services.AddScoped<IElectionAccessor>(_ => new ElectionAccessor(connectionString));
builder.Services.AddScoped<IVoteAccessor>(_ => new VoteAccessor(connectionString));

// engines (pure logic, no state, safe as singleton)
builder.Services.AddSingleton<IAuthEngine, AuthEngine>();
builder.Services.AddSingleton<IBallotValidationEngine, BallotValidationEngine>();
builder.Services.AddSingleton<IDuplicateVoteEngine, DuplicateVoteEngine>();
builder.Services.AddSingleton<IResultsEngine, ResultsEngine>();

// managers (orchestration)
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IBallotManager, BallotManager>();
builder.Services.AddScoped<IVotingSessionManager, VotingSessionManager>();
builder.Services.AddScoped<IResultsManager, ResultsManager>();
builder.Services.AddScoped<IVoteLookupManager, VoteLookupManager>();
builder.Services.AddScoped<IParticipationManager, ParticipationManager>();
builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();

var app = builder.Build();

app.MapGet("/", () => "Pacopolis Voting System API");

// POST /api/login - takes LoginRequest, returns LoginResponse
// always returns 200, success/failure in the response body
app.MapPost("/api/login", async (LoginRequest request, IAuthManager authManager) =>
{
    var response = await authManager.LoginAsync(request);
    return Results.Ok(response);
});

// POST /api/register - takes RegisterRequest, returns RegisterResponse
// always returns 200, success/failure in the response body (same pattern as login)
app.MapPost("/api/register", async (RegisterRequest request, IRegistrationManager registrationManager) =>
{
    var response = await registrationManager.RegisterAsync(request);
    return Results.Ok(response);
});

// GET /api/ballot - returns the active ballot, 404 if no election is active
app.MapGet("/api/ballot", async (IBallotManager ballotManager) =>
{
    var ballot = await ballotManager.GetActiveBallotAsync();
    return ballot is null ? Results.NotFound() : Results.Ok(ballot);
});

// POST /api/submit-ballot - takes SubmitBallotRequest, returns SubmitBallotResponse
// always returns 200, success/failure in the response body
app.MapPost("/api/submit-ballot", async (SubmitBallotRequest request, IVotingSessionManager sessionManager) =>
{
    var response = await sessionManager.SubmitBallotAsync(request);
    return Results.Ok(response);
});

// GET /api/results - returns election results with vote counts and winners
app.MapGet("/api/results", async (IResultsManager resultsManager) =>
{
    var results = await resultsManager.GetResultsAsync();
    return results is null ? Results.NotFound("No election found.") : Results.Ok(results);
});

// GET /api/vote-lookup/{code} - voter verifies their recorded selections by confirmation code
// 404 if the code is not found
app.MapGet("/api/vote-lookup/{code:guid}", async (Guid code, IVoteLookupManager lookupManager) =>
{
    var dto = await lookupManager.GetByConfirmationCodeAsync(code);
    return dto is null ? Results.NotFound("Confirmation code not found.") : Results.Ok(dto);
});

// GET /api/participation?username=... - third-party check of voter participation (voted yes/no only)
// 400 if username missing; otherwise 200 with { voted: bool }
app.MapGet("/api/participation", async (string? username, IParticipationManager participationManager) =>
{
    if (string.IsNullOrWhiteSpace(username))
        return Results.BadRequest(new { message = "Missing 'username' query parameter." });

    var dto = await participationManager.CheckAsync(username);
    return Results.Ok(dto);
});

// DEV ONLY: reset votes for demo purposes
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/dev/reset-votes", async (IVoteAccessor voteAccessor) =>
    {
        // uses the connection string already registered
        var connStr = builder.Configuration.GetConnectionString("VotingDb")!;
        using var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr);
        await conn.OpenAsync();
        await conn.ExecuteAsync("DELETE FROM Vote; DELETE FROM VoterRecord;");
        return Results.Ok(new { success = true, message = "All votes cleared." });
    });
}

app.Run();
