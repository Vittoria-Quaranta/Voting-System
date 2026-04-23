using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Determines winners from raw vote counts.
/// No database access, just math and sorting.
/// </summary>
public class ResultsEngine : IResultsEngine
{
    public RaceResultDto BuildRaceResult(Race race, IEnumerable<Candidate> candidates, IEnumerable<VoteCount> voteCounts)
    {
        var countsByCandidate = voteCounts
            .Where(vc => vc.RaceId == race.RaceId)
            .ToDictionary(vc => vc.CandidateId, vc => vc.Count);

        var totalVotes = countsByCandidate.Values.Sum();
        var maxVotes = countsByCandidate.Values.DefaultIfEmpty(0).Max();

        var candidateResults = candidates.Select(c =>
        {
            var votes = countsByCandidate.GetValueOrDefault(c.CandidateId, 0);
            return new CandidateResultDto
            {
                CandidateId = c.CandidateId,
                CandidateName = c.CandidateName,
                Party = c.Party,
                Votes = votes,
                Percentage = totalVotes > 0 ? Math.Round((double)votes / totalVotes * 100, 1) : 0,
                IsWinner = votes > 0 && votes == maxVotes
            };
        })
        .OrderByDescending(c => c.Votes)
        .ToList();

        return new RaceResultDto
        {
            RaceId = race.RaceId,
            RaceName = race.RaceName,
            RaceType = race.RaceType,
            TotalVotes = totalVotes,
            Candidates = candidateResults
        };
    }
}
