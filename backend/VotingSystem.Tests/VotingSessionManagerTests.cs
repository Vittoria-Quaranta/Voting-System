using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class VotingSessionManagerTests
{
    private class FakeBallotManager : IBallotManager
    {
        public BallotDto? BallotToReturn { get; set; }
        public Task<BallotDto?> GetActiveBallotAsync() => Task.FromResult(BallotToReturn);
    }

    private class FakeVoterAccessor : IVoterAccessor
    {
        public bool HasVoted { get; set; }
        public Task<Voter?> GetVoterByUsernameAsync(string username) => Task.FromResult<Voter?>(null);
        public Task<bool> HasVotedInElectionAsync(int voterId, int electionId) => Task.FromResult(HasVoted);
    }

    private class FakeValidationEngine : IBallotValidationEngine
    {
        public ValidationResult ResultToReturn { get; set; } = ValidationResult.Valid();
        public ValidationResult Validate(BallotDto ballot, List<SelectionDto> selections) => ResultToReturn;
    }

    private class FakeDuplicateVoteEngine : IDuplicateVoteEngine
    {
        public ValidationResult ResultToReturn { get; set; } = ValidationResult.Valid();
        public ValidationResult Check(bool hasAlreadyVoted) => ResultToReturn;
    }

    private class FakeVoteAccessor : IVoteAccessor
    {
        public Guid CodeToReturn { get; set; } = Guid.NewGuid();
        public Task<Guid> SubmitBallotAsync(int voterId, int electionId, List<Vote> selections) => Task.FromResult(CodeToReturn);
        public Task<IEnumerable<Vote>> GetVotesByConfirmationCodeAsync(Guid confirmationCode) =>
            Task.FromResult<IEnumerable<Vote>>(new List<Vote>());
    }

    private static BallotDto MakeBallot() => new()
    {
        ElectionId = 1,
        ElectionName = "Test",
        Races = new List<RaceDto>
        {
            new RaceDto { RaceId = 10, RaceName = "Mayor", RaceType = "Candidate" }
        }
    };

    private static SubmitBallotRequest MakeRequest() => new()
    {
        VoterId = 1,
        ElectionId = 1,
        Selections = new List<SelectionDto> { new SelectionDto { RaceId = 10, ChoiceId = 100 } }
    };

    [TestMethod]
    public async Task SubmitBallotAsync_ValidSubmission_ReturnsSuccessWithConfirmationCode()
    {
        var expectedCode = Guid.NewGuid();
        var manager = new VotingSessionManager(
            new FakeBallotManager { BallotToReturn = MakeBallot() },
            new FakeVoterAccessor { HasVoted = false },
            new FakeDuplicateVoteEngine { ResultToReturn = ValidationResult.Valid() },
            new FakeValidationEngine { ResultToReturn = ValidationResult.Valid() },
            new FakeVoteAccessor { CodeToReturn = expectedCode }
        );

        var response = await manager.SubmitBallotAsync(MakeRequest());

        Assert.IsTrue(response.Success);
        Assert.AreEqual(expectedCode, response.ConfirmationCode);
    }

    [TestMethod]
    public async Task SubmitBallotAsync_NoActiveElection_ReturnsFailure()
    {
        var manager = new VotingSessionManager(
            new FakeBallotManager { BallotToReturn = null },
            new FakeVoterAccessor(),
            new FakeDuplicateVoteEngine(),
            new FakeValidationEngine(),
            new FakeVoteAccessor()
        );

        var response = await manager.SubmitBallotAsync(MakeRequest());

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Message.Contains("No active"));
    }

    [TestMethod]
    public async Task SubmitBallotAsync_AlreadyVoted_ReturnsFailure()
    {
        var manager = new VotingSessionManager(
            new FakeBallotManager { BallotToReturn = MakeBallot() },
            new FakeVoterAccessor { HasVoted = true },
            new FakeDuplicateVoteEngine { ResultToReturn = ValidationResult.Invalid("You have already voted in this election.") },
            new FakeValidationEngine(),
            new FakeVoteAccessor()
        );

        var response = await manager.SubmitBallotAsync(MakeRequest());

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Message.Contains("already voted"));
    }

    [TestMethod]
    public async Task SubmitBallotAsync_InvalidBallot_ReturnsFailure()
    {
        var manager = new VotingSessionManager(
            new FakeBallotManager { BallotToReturn = MakeBallot() },
            new FakeVoterAccessor { HasVoted = false },
            new FakeDuplicateVoteEngine(),
            new FakeValidationEngine { ResultToReturn = ValidationResult.Invalid("Missing selection.") },
            new FakeVoteAccessor()
        );

        var response = await manager.SubmitBallotAsync(MakeRequest());

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Missing selection.", response.Message);
    }
}
