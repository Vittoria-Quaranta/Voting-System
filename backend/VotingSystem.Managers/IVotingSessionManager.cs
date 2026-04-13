using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Handles the submit ballot flow.
/// </summary>
public interface IVotingSessionManager
{
    /// <summary>
    /// Submit a finished ballot. Validates, checks for duplicate voting, and saves.
    /// </summary>
    Task<SubmitBallotResponse> SubmitBallotAsync(SubmitBallotRequest request);
}
