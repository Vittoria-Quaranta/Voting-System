using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Orchestrates the login flow.
/// </summary>
public interface IAuthManager
{
    /// <summary>
    /// Try to log in a voter. Returns success with voter info or failure with a message.
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
