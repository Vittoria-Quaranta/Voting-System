using VotingSystem.DataContracts;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Resolves a confirmation code into a human-readable list of the voter's selections.
/// Returns null when the code does not exist so the API can map that to 404.
/// </summary>
public class VoteLookupManager : IVoteLookupManager
{
    private readonly IVoteAccessor _voteAccessor;
    private readonly IElectionAccessor _electionAccessor;

    public VoteLookupManager(IVoteAccessor voteAccessor, IElectionAccessor electionAccessor)
    {
        _voteAccessor = voteAccessor;
        _electionAccessor = electionAccessor;
    }

    public async Task<VoteLookupDto?> GetByConfirmationCodeAsync(Guid confirmationCode)
    {
        // step 1: find which election this code belongs to
        var record = await _voteAccessor.GetVoterRecordByConfirmationCodeAsync(confirmationCode);
        if (record == null)
            return null;

        // step 2: pull the actual vote rows (RaceId + CandidateId pairs)
        var votes = (await _voteAccessor.GetVotesByConfirmationCodeAsync(confirmationCode)).ToList();

        // step 3: pull the election metadata for the human-readable name
        var races = (await _electionAccessor.GetRacesByElectionAsync(record.ElectionId)).ToList();
        var raceMap = races.ToDictionary(r => r.RaceId);

        // step 4: need candidate names per race; one call per race keeps the accessor simple
        var result = new VoteLookupDto
        {
            ElectionId = record.ElectionId,
            ElectionName = await GetElectionNameAsync(record.ElectionId)
        };

        foreach (var vote in votes)
        {
            if (!raceMap.TryGetValue(vote.RaceId, out var race))
                continue; // race was deleted after vote was cast; skip silently

            var candidates = await _electionAccessor.GetCandidatesByRaceAsync(vote.RaceId);
            var candidate = candidates.FirstOrDefault(c => c.CandidateId == vote.CandidateId);

            result.Selections.Add(new VoteLookupItemDto
            {
                RaceId = race.RaceId,
                RaceName = race.RaceName,
                RaceType = race.RaceType,
                CandidateId = vote.CandidateId,
                CandidateName = candidate?.CandidateName ?? "(unknown)",
                Party = candidate?.Party
            });
        }

        return result;
    }

    // active election is the one we know about; if the stored election is no longer
    // active we still want its name, so fall back to ElectionId as a string label
    private async Task<string> GetElectionNameAsync(int electionId)
    {
        var active = await _electionAccessor.GetActiveElectionAsync();
        if (active != null && active.ElectionId == electionId)
            return active.ElectionName;
        return $"Election {electionId}";
    }
}
