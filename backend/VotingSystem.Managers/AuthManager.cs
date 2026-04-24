using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Handles the login flow by coordinating the voter accessor and auth engine.
/// </summary>
public class AuthManager : IAuthManager
{
    private readonly IVoterAccessor _voterAccessor;
    private readonly IElectionAccessor _electionAccessor;
    private readonly IAuthEngine _authEngine;

    // generic message for both "no such user" and "wrong password"
    // so attackers cannot tell which usernames exist
    private const string InvalidLoginMessage = "Invalid username or password.";

    public AuthManager(IVoterAccessor voterAccessor, IElectionAccessor electionAccessor, IAuthEngine authEngine)
    {
        _voterAccessor = voterAccessor;
        _electionAccessor = electionAccessor;
        _authEngine = authEngine;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // guard: empty username or password fails immediately, no DB hit needed
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse { Success = false, Message = InvalidLoginMessage };
        }

        // step 1: pull the voter from the database
        var voter = await _voterAccessor.GetVoterByUsernameAsync(request.Username);
        if (voter == null)
        {
            return new LoginResponse { Success = false, Message = InvalidLoginMessage };
        }

        // step 2: verify the password matches the stored hash
        if (!_authEngine.VerifyPassword(request.Password, voter.PasswordHash))
        {
            return new LoginResponse { Success = false, Message = InvalidLoginMessage };
        }

        // step 3: check if they already voted in the active election
        var hasVoted = false;
        var election = await _electionAccessor.GetActiveElectionAsync();
        if (election != null)
        {
            hasVoted = await _voterAccessor.HasVotedInElectionAsync(voter.VoterId, election.ElectionId);
        }

        // step 4: success, return voter info for the session
        return new LoginResponse
        {
            Success = true,
            Message = "Login successful.",
            VoterId = voter.VoterId,
            FirstName = voter.FirstName,
            LastName = voter.LastName,
            HasVoted = hasVoted
        };
    }
}
