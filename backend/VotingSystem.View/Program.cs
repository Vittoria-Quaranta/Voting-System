using VotingSystem.ResourceAccess;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("VotingDb")
    ?? throw new InvalidOperationException(
        "Missing 'VotingDb' connection string. " +
        "Copy appsettings.Development.json.template to appsettings.Development.json and fill in your credentials.");

builder.Services.AddScoped<IVoterAccessor>(_ => new VoterAccessor(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Pacopolis Voting System API");

app.Run();
