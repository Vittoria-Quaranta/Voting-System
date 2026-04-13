using VotingSystem.DataContracts;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Assembles the active ballot by pulling the election, races, and choices from the accessor.
/// </summary>
public class BallotManager : IBallotManager
{
    private readonly IElectionAccessor _electionAccessor;

    public BallotManager(IElectionAccessor electionAccessor)
    {
        _electionAccessor = electionAccessor;
    }

    public async Task<BallotDto?> GetActiveBallotAsync()
    {
        // step 1: find the active election, bail if there is none
        var election = await _electionAccessor.GetActiveElectionAsync();
        if (election == null)
        {
            return null;
        }

        // step 2: get the races for this election
        var races = await _electionAccessor.GetRacesByElectionAsync(election.ElectionId);

        // step 3: build the ballot, loading choices for each race as we go
        var ballot = new BallotDto
        {
            ElectionId = election.ElectionId,
            ElectionName = election.ElectionName
        };

        foreach (var race in races)
        {
            var candidates = await _electionAccessor.GetCandidatesByRaceAsync(race.RaceId);

            var raceDto = new RaceDto
            {
                RaceId = race.RaceId,
                RaceName = race.RaceName,
                RaceType = race.RaceType
            };

            foreach (var candidate in candidates)
            {
                raceDto.Choices.Add(new ChoiceDto
                {
                    ChoiceId = candidate.CandidateId,
                    ChoiceName = candidate.CandidateName,
                    Party = candidate.Party
                });
            }

            ballot.Races.Add(raceDto);
        }

        return ballot;
    }
}
