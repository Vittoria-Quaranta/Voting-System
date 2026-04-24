using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Pulls election data from accessors, runs it through the results engine,
/// and assembles the full results response.
/// </summary>
public class ResultsManager : IResultsManager
{
    private readonly IElectionAccessor _electionAccessor;
    private readonly IVoteAccessor _voteAccessor;
    private readonly IResultsEngine _resultsEngine;

    public ResultsManager(
        IElectionAccessor electionAccessor,
        IVoteAccessor voteAccessor,
        IResultsEngine resultsEngine)
    {
        _electionAccessor = electionAccessor;
        _voteAccessor = voteAccessor;
        _resultsEngine = resultsEngine;
    }

    public async Task<ResultsDto?> GetResultsAsync()
    {
        // step 1: find the active election
        var election = await _electionAccessor.GetActiveElectionAsync();
        if (election == null)
            return null;

        // step 2: get all races and vote counts
        var races = (await _electionAccessor.GetRacesByElectionAsync(election.ElectionId)).ToList();
        var voteCounts = (await _voteAccessor.GetVoteCountsByElectionAsync(election.ElectionId)).ToList();

        // step 3: build results for each race using the engine
        var results = new ResultsDto
        {
            ElectionId = election.ElectionId,
            ElectionName = election.ElectionName
        };

        foreach (var race in races)
        {
            var candidates = await _electionAccessor.GetCandidatesByRaceAsync(race.RaceId);
            var raceResult = _resultsEngine.BuildRaceResult(race, candidates, voteCounts);
            results.Races.Add(raceResult);
        }

        return results;
    }
}
