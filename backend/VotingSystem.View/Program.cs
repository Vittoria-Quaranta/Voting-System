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

// managers (orchestration)
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IBallotManager, BallotManager>();
builder.Services.AddScoped<IVotingSessionManager, VotingSessionManager>();

var app = builder.Build();

app.MapGet("/", () => "Pacopolis Voting System API");

// POST /api/login - takes LoginRequest, returns LoginResponse
// always returns 200, success/failure in the response body
app.MapPost("/api/login", async (LoginRequest request, IAuthManager authManager) =>
{
    var response = await authManager.LoginAsync(request);
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

app.Run();
