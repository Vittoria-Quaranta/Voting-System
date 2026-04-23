using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Data access for saving and retrieving votes.
/// </summary>
public interface IVoteAccessor
{
    /// <summary>
    /// Submit a full ballot. Saves the voter record and all selections in one transaction.
    /// Returns the confirmation code the voter can use to look up their ballot later.
    /// </summary>
    Task<Guid> SubmitBallotAsync(int voterId, int electionId, List<Vote> selections);

    /// <summary>
    /// Look up submitted votes by confirmation code.
    /// </summary>
    Task<IEnumerable<Vote>> GetVotesByConfirmationCodeAsync(Guid confirmationCode);

    /// <summary>
    /// Look up the VoterRecord row for a confirmation code.
    /// Used by the lookup flow to resolve which election the code belongs to.
    /// Returns null if the code does not exist.
    /// </summary>
    Task<VoterRecord?> GetVoterRecordByConfirmationCodeAsync(Guid confirmationCode);

    /// <summary>
    /// Get vote counts grouped by race and candidate for an election.
    /// </summary>
    Task<IEnumerable<VoteCount>> GetVoteCountsByElectionAsync(int electionId);
}
