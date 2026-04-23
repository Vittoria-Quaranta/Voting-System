using Microsoft.Data.SqlClient;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Validates a registration request, hashes the password, and inserts the voter.
/// Username duplication is checked before insert for a friendly error, and caught
/// at the DB level as a backstop for the TOCTOU window between check and insert.
/// </summary>
public class RegistrationManager : IRegistrationManager
{
    private const int MinPasswordLength = 8;
    private const string UsernameTakenMessage = "Username already taken.";

    private readonly IVoterAccessor _voterAccessor;
    private readonly IAuthEngine _authEngine;

    public RegistrationManager(IVoterAccessor voterAccessor, IAuthEngine authEngine)
    {
        _voterAccessor = voterAccessor;
        _authEngine = authEngine;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var username = request.Username?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName))
            return Fail("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            return Fail("Last name is required.");
        if (string.IsNullOrWhiteSpace(username))
            return Fail("Username is required.");
        if (string.IsNullOrWhiteSpace(password))
            return Fail("Password is required.");
        if (password.Length < MinPasswordLength)
            return Fail($"Password must be at least {MinPasswordLength} characters.");

        var existing = await _voterAccessor.GetVoterByUsernameAsync(username);
        if (existing != null)
            return Fail(UsernameTakenMessage);

        var voter = new Voter
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            PasswordHash = _authEngine.HashPassword(password),
            DateOfBirth = request.DateOfBirth
        };

        int newVoterId;
        try
        {
            newVoterId = await _voterAccessor.CreateVoterAsync(voter);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            // race: someone else registered the same username between our check and this insert
            return Fail(UsernameTakenMessage);
        }

        return new RegisterResponse
        {
            Success = true,
            Message = "Registration successful.",
            VoterId = newVoterId,
            FirstName = firstName,
            LastName = lastName
        };
    }

    private static RegisterResponse Fail(string message) =>
        new() { Success = false, Message = message };
}
