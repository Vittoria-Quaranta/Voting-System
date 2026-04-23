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

    /// <summary>
    /// Insert a new Voter row. Returns the new VoterId.
    /// Assumes the caller has already hashed the password and checked the username is free.
    /// The DB's UNIQUE constraint on Username still guards against a race.
    /// </summary>
    Task<int> CreateVoterAsync(Voter voter);
}
