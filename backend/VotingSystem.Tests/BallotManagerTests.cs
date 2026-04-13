using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class BallotManagerTests
{
    // fake election accessor, returns whatever we set on it
    private class FakeElectionAccessor : IElectionAccessor
    {
        public Election? ActiveElection { get; set; }
        public Dictionary<int, List<Race>> RacesByElection { get; set; } = new();
        public Dictionary<int, List<Candidate>> CandidatesByRace { get; set; } = new();

        public Task<Election?> GetActiveElectionAsync() => Task.FromResult(ActiveElection);

        public Task<IEnumerable<Race>> GetRacesByElectionAsync(int electionId)
        {
            var races = RacesByElection.TryGetValue(electionId, out var list) ? list : new List<Race>();
            return Task.FromResult<IEnumerable<Race>>(races);
        }

        public Task<IEnumerable<Candidate>> GetCandidatesByRaceAsync(int raceId)
        {
            var cands = CandidatesByRace.TryGetValue(raceId, out var list) ? list : new List<Candidate>();
            return Task.FromResult<IEnumerable<Candidate>>(cands);
        }
    }

    [TestMethod]
    public async Task GetActiveBallotAsync_NoActiveElection_ReturnsNull()
    {
        var fakeAccessor = new FakeElectionAccessor { ActiveElection = null };
        var manager = new BallotManager(fakeAccessor);

        var ballot = await manager.GetActiveBallotAsync();

        Assert.IsNull(ballot);
    }

    [TestMethod]
    public async Task GetActiveBallotAsync_ElectionExists_ReturnsBallotWithCorrectStructure()
    {
        // arrange: one election, one race, two candidates
        var fakeAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "Test Election" },
            RacesByElection = new Dictionary<int, List<Race>>
            {
                [1] = new List<Race>
                {
                    new Race { RaceId = 10, ElectionId = 1, RaceName = "Mayor", RaceType = "Candidate" }
                }
            },
            CandidatesByRace = new Dictionary<int, List<Candidate>>
            {
                [10] = new List<Candidate>
                {
                    new Candidate { CandidateId = 100, RaceId = 10, CandidateName = "Lil Red", Party = "Spirit" },
                    new Candidate { CandidateId = 101, RaceId = 10, CandidateName = "Herbie Husker", Party = "Tradition" }
                }
            }
        };
        var manager = new BallotManager(fakeAccessor);

        var ballot = await manager.GetActiveBallotAsync();

        Assert.IsNotNull(ballot);
        Assert.AreEqual(1, ballot.Races.Count);
        Assert.AreEqual(2, ballot.Races[0].Choices.Count);
    }

    [TestMethod]
    public async Task GetActiveBallotAsync_ResponseShape_HasAllFieldsPopulated()
    {
        var fakeAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 Pacopolis General Election" },
            RacesByElection = new Dictionary<int, List<Race>>
            {
                [1] = new List<Race>
                {
                    new Race { RaceId = 10, ElectionId = 1, RaceName = "Mayor", RaceType = "Candidate" }
                }
            },
            CandidatesByRace = new Dictionary<int, List<Candidate>>
            {
                [10] = new List<Candidate>
                {
                    new Candidate { CandidateId = 100, RaceId = 10, CandidateName = "Lil Red", Party = "Spirit Party" }
                }
            }
        };
        var manager = new BallotManager(fakeAccessor);

        var ballot = await manager.GetActiveBallotAsync();

        // election fields
        Assert.IsNotNull(ballot);
        Assert.AreEqual(1, ballot.ElectionId);
        Assert.AreEqual("2026 Pacopolis General Election", ballot.ElectionName);

        // race fields
        var race = ballot.Races[0];
        Assert.AreEqual(10, race.RaceId);
        Assert.AreEqual("Mayor", race.RaceName);
        Assert.AreEqual("Candidate", race.RaceType);

        // choice fields
        var choice = race.Choices[0];
        Assert.AreEqual(100, choice.ChoiceId);
        Assert.AreEqual("Lil Red", choice.ChoiceName);
        Assert.AreEqual("Spirit Party", choice.Party);
    }
}
