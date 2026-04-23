using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class ParticipationManagerTests
{
    // fake voter accessor: configurable voter + voted flag
    private class FakeVoterAccessor : IVoterAccessor
    {
        public Voter? VoterToReturn { get; set; }
        public bool HasVotedResult { get; set; }

        public Task<Voter?> GetVoterByUsernameAsync(string username) => Task.FromResult(VoterToReturn);
        public Task<bool> HasVotedInElectionAsync(int voterId, int electionId) => Task.FromResult(HasVotedResult);
    }

    // fake election accessor: only GetActiveElectionAsync is exercised here, others stub empty
    private class FakeElectionAccessor : IElectionAccessor
    {
        public Election? ActiveElection { get; set; }

        public Task<Election?> GetActiveElectionAsync() => Task.FromResult(ActiveElection);
        public Task<IEnumerable<Race>> GetRacesByElectionAsync(int electionId) =>
            Task.FromResult<IEnumerable<Race>>(new List<Race>());
        public Task<IEnumerable<Candidate>> GetCandidatesByRaceAsync(int raceId) =>
            Task.FromResult<IEnumerable<Candidate>>(new List<Candidate>());
    }

    [TestMethod]
    public async Task CheckAsync_UnknownUsername_ReturnsVotedFalse()
    {
        var voterAccessor = new FakeVoterAccessor { VoterToReturn = null };
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" }
        };
        var manager = new ParticipationManager(voterAccessor, electionAccessor);

        var result = await manager.CheckAsync("nobody");

        Assert.IsFalse(result.Voted);
    }

    [TestMethod]
    public async Task CheckAsync_NoActiveElection_ReturnsVotedFalse()
    {
        var voterAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier" },
            HasVotedResult = true  // even if this were true, no active election means false
        };
        var electionAccessor = new FakeElectionAccessor { ActiveElection = null };
        var manager = new ParticipationManager(voterAccessor, electionAccessor);

        var result = await manager.CheckAsync("tfrazier");

        Assert.IsFalse(result.Voted);
    }

    [TestMethod]
    public async Task CheckAsync_VoterHasVoted_ReturnsVotedTrue()
    {
        var voterAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier" },
            HasVotedResult = true
        };
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" }
        };
        var manager = new ParticipationManager(voterAccessor, electionAccessor);

        var result = await manager.CheckAsync("tfrazier");

        Assert.IsTrue(result.Voted);
    }

    [TestMethod]
    public async Task CheckAsync_VoterHasNotVoted_ReturnsVotedFalse()
    {
        var voterAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier" },
            HasVotedResult = false
        };
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" }
        };
        var manager = new ParticipationManager(voterAccessor, electionAccessor);

        var result = await manager.CheckAsync("tfrazier");

        Assert.IsFalse(result.Voted);
    }
}
