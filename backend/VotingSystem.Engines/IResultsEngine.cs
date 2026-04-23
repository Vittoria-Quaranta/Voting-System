using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Pure logic for determining election results.
/// Takes raw vote counts and candidate info, returns results with winners marked.
/// </summary>
public interface IResultsEngine
{
    /// <summary>
    /// Build results for a single race. Calculates percentages and marks the winner.
    /// </summary>
    RaceResultDto BuildRaceResult(Race race, IEnumerable<Candidate> candidates, IEnumerable<VoteCount> voteCounts);
}
