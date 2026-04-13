using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Managers;

/// <summary>
/// Coordinates the submit ballot flow.
/// Loads the ballot, checks for duplicate voting, validates, saves.
/// </summary>
public class VotingSessionManager : IVotingSessionManager
{
    private readonly IBallotManager _ballotManager;
    private readonly IVoterAccessor _voterAccessor;
    private readonly IBallotValidationEngine _validationEngine;
    private readonly IVoteAccessor _voteAccessor;

    public VotingSessionManager(
        IBallotManager ballotManager,
        IVoterAccessor voterAccessor,
        IBallotValidationEngine validationEngine,
        IVoteAccessor voteAccessor)
    {
        _ballotManager = ballotManager;
        _voterAccessor = voterAccessor;
        _validationEngine = validationEngine;
        _voteAccessor = voteAccessor;
    }

    public async Task<SubmitBallotResponse> SubmitBallotAsync(SubmitBallotRequest request)
    {
        // step 1: make sure there is an active ballot to submit against
        var ballot = await _ballotManager.GetActiveBallotAsync();
        if (ballot == null)
        {
            return Fail("No active election to submit to.");
        }

        // step 2: prevent duplicate voting
        var alreadyVoted = await _voterAccessor.HasVotedInElectionAsync(request.VoterId, request.ElectionId);
        if (alreadyVoted)
        {
            return Fail("You have already voted in this election.");
        }

        // step 3: validate the selections against the ballot
        var validation = _validationEngine.Validate(ballot, request.Selections);
        if (!validation.IsValid)
        {
            return Fail(validation.ErrorMessage);
        }

        // step 4: map selections to Vote entities and save
        var votes = request.Selections.Select(s => new Vote
        {
            RaceId = s.RaceId,
            CandidateId = s.ChoiceId
        }).ToList();

        var confirmationCode = await _voteAccessor.SubmitBallotAsync(request.VoterId, request.ElectionId, votes);

        return new SubmitBallotResponse
        {
            Success = true,
            Message = "Ballot submitted successfully.",
            ConfirmationCode = confirmationCode
        };
    }

    private static SubmitBallotResponse Fail(string message) =>
        new() { Success = false, Message = message };
}
