using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Data access for Election, Race, and Candidate tables.
/// </summary>
public interface IElectionAccessor
{
    /// <summary>
    /// Get the currently active election, or null if none are active.
    /// </summary>
    Task<Election?> GetActiveElectionAsync();

    /// <summary>
    /// Get all races for a given election.
    /// </summary>
    Task<IEnumerable<Race>> GetRacesByElectionAsync(int electionId);

    /// <summary>
    /// Get all candidates/options for a given race.
    /// </summary>
    Task<IEnumerable<Candidate>> GetCandidatesByRaceAsync(int raceId);
}
