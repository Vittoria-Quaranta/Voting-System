using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Data access for the Voter and VoterRecord tables.
/// </summary>
public interface IVoterAccessor
{
    /// <summary>
    /// Look up a voter by username. Returns null if not found.
    /// </summary>
    Task<Voter?> GetVoterByUsernameAsync(string username);

    /// <summary>
    /// Check if a voter has already voted in a given election.
    /// </summary>
    Task<bool> HasVotedInElectionAsync(int voterId, int electionId);
}
