using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Handles new voter sign-up: validates input, hashes the password,
/// and creates the Voter row.
/// </summary>
public interface IRegistrationManager
{
    /// <summary>
    /// Create a new voter account. Always returns a response, never throws for
    /// validation or duplicate-username errors, those show up as Success = false.
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}
