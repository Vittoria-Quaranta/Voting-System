using VotingSystem.DataContracts;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Checks voter participation without disclosing choices.
/// Returns a uniform shape so unknown-username and has-not-voted look identical to the caller.
/// </summary>
public class ParticipationManager : IParticipationManager
{
    private readonly IVoterAccessor _voterAccessor;
    private readonly IElectionAccessor _electionAccessor;

    public ParticipationManager(IVoterAccessor voterAccessor, IElectionAccessor electionAccessor)
    {
        _voterAccessor = voterAccessor;
        _electionAccessor = electionAccessor;
    }

    public async Task<ParticipationDto> CheckAsync(string username)
    {
        var voter = await _voterAccessor.GetVoterByUsernameAsync(username);
        if (voter == null)
            return new ParticipationDto { Voted = false };

        var election = await _electionAccessor.GetActiveElectionAsync();
        if (election == null)
            return new ParticipationDto { Voted = false };

        var voted = await _voterAccessor.HasVotedInElectionAsync(voter.VoterId, election.ElectionId);
        return new ParticipationDto { Voted = voted };
    }
}
