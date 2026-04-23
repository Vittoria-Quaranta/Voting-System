using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Orchestrates building election results from accessor data and engine logic.
/// </summary>
public interface IResultsManager
{
    /// <summary>
    /// Get results for the active election.
    /// Returns null if no election exists, or a message if the election is still open.
    /// </summary>
    Task<ResultsDto?> GetResultsAsync();
}
