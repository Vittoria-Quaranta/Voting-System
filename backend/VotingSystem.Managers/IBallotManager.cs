using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Builds the active ballot for display.
/// </summary>
public interface IBallotManager
{
    /// <summary>
    /// Returns the currently active ballot with all races and choices.
    /// Returns null if no election is active.
    /// </summary>
    Task<BallotDto?> GetActiveBallotAsync();
}
