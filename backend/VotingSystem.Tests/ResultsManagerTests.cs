using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class ResultsManagerTests
{
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

    private class FakeVoteAccessor : IVoteAccessor
    {
        public List<VoteCount> VoteCounts { get; set; } = new();

        public Task<Guid> SubmitBallotAsync(int voterId, int electionId, List<Vote> selections) =>
            Task.FromResult(Guid.Empty);
        public Task<IEnumerable<Vote>> GetVotesByConfirmationCodeAsync(Guid confirmationCode) =>
            Task.FromResult<IEnumerable<Vote>>(new List<Vote>());
        public Task<IEnumerable<VoteCount>> GetVoteCountsByElectionAsync(int electionId) =>
            Task.FromResult<IEnumerable<VoteCount>>(VoteCounts);
        public Task<VoterRecord?> GetVoterRecordByConfirmationCodeAsync(Guid confirmationCode) =>
            Task.FromResult<VoterRecord?>(null);
    }

    private static ResultsManager MakeManager(FakeElectionAccessor electionAccessor, FakeVoteAccessor voteAccessor)
        => new(electionAccessor, voteAccessor, new ResultsEngine());

    [TestMethod]
    public async Task GetResultsAsync_NoActiveElection_ReturnsNull()
    {
        var manager = MakeManager(new FakeElectionAccessor { ActiveElection = null }, new FakeVoteAccessor());

        var result = await manager.GetResultsAsync();

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetResultsAsync_OneRaceWithVotes_ReturnsDtoWithWinner()
    {
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" },
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
                    new Candidate { CandidateId = 101, RaceId = 10, CandidateName = "Herbie", Party = "Tradition" }
                }
            }
        };
        var voteAccessor = new FakeVoteAccessor
        {
            VoteCounts = new List<VoteCount>
            {
                new() { RaceId = 10, CandidateId = 100, Count = 7 },
                new() { RaceId = 10, CandidateId = 101, Count = 3 }
            }
        };
        var manager = MakeManager(electionAccessor, voteAccessor);

        var result = await manager.GetResultsAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.ElectionId);
        Assert.AreEqual("2026 General", result.ElectionName);
        Assert.AreEqual(1, result.Races.Count);

        var mayor = result.Races[0];
        Assert.AreEqual(10, mayor.RaceId);
        Assert.AreEqual("Mayor", mayor.RaceName);
        Assert.AreEqual(10, mayor.TotalVotes);

        var winner = mayor.Candidates.First(c => c.IsWinner);
        Assert.AreEqual("Lil Red", winner.CandidateName);
        Assert.AreEqual(7, winner.Votes);
    }

    [TestMethod]
    public async Task GetResultsAsync_MultipleRaces_EachRaceHasItsOwnCandidates()
    {
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" },
            RacesByElection = new Dictionary<int, List<Race>>
            {
                [1] = new List<Race>
                {
                    new Race { RaceId = 10, ElectionId = 1, RaceName = "Mayor", RaceType = "Candidate" },
                    new Race { RaceId = 20, ElectionId = 1, RaceName = "Prop 1", RaceType = "YesNo" }
                }
            },
            CandidatesByRace = new Dictionary<int, List<Candidate>>
            {
                [10] = new List<Candidate>
                {
                    new Candidate { CandidateId = 100, RaceId = 10, CandidateName = "Lil Red" },
                    new Candidate { CandidateId = 101, RaceId = 10, CandidateName = "Herbie" }
                },
                [20] = new List<Candidate>
                {
                    new Candidate { CandidateId = 200, RaceId = 20, CandidateName = "Yes" },
                    new Candidate { CandidateId = 201, RaceId = 20, CandidateName = "No" }
                }
            }
        };
        var voteAccessor = new FakeVoteAccessor
        {
            VoteCounts = new List<VoteCount>
            {
                new() { RaceId = 10, CandidateId = 100, Count = 5 },
                new() { RaceId = 20, CandidateId = 200, Count = 8 }
            }
        };
        var manager = MakeManager(electionAccessor, voteAccessor);

        var result = await manager.GetResultsAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result!.Races.Count);

        var mayor = result.Races.First(r => r.RaceId == 10);
        Assert.AreEqual(2, mayor.Candidates.Count);
        Assert.IsTrue(mayor.Candidates.All(c => c.CandidateName == "Lil Red" || c.CandidateName == "Herbie"));

        var prop = result.Races.First(r => r.RaceId == 20);
        Assert.AreEqual(2, prop.Candidates.Count);
        Assert.IsTrue(prop.Candidates.All(c => c.CandidateName == "Yes" || c.CandidateName == "No"));
    }

    [TestMethod]
    public async Task GetResultsAsync_NoVotes_ReturnsZeroTotalVotes()
    {
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" },
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
                    new Candidate { CandidateId = 100, RaceId = 10, CandidateName = "Lil Red" }
                }
            }
        };
        var manager = MakeManager(electionAccessor, new FakeVoteAccessor());

        var result = await manager.GetResultsAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result!.Races[0].TotalVotes);
        Assert.IsFalse(result.Races[0].Candidates[0].IsWinner);
    }
}
