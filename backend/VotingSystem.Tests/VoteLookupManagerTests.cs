using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class VoteLookupManagerTests
{
    // fake vote accessor: returns whatever VoterRecord and votes we hand it
    private class FakeVoteAccessor : IVoteAccessor
    {
        public VoterRecord? RecordToReturn { get; set; }
        public List<Vote> VotesToReturn { get; set; } = new();

        public Task<Guid> SubmitBallotAsync(int voterId, int electionId, List<Vote> selections) =>
            Task.FromResult(Guid.Empty);

        public Task<IEnumerable<Vote>> GetVotesByConfirmationCodeAsync(Guid confirmationCode) =>
            Task.FromResult<IEnumerable<Vote>>(VotesToReturn);

        public Task<IEnumerable<VoteCount>> GetVoteCountsByElectionAsync(int electionId) =>
            Task.FromResult<IEnumerable<VoteCount>>(new List<VoteCount>());

        public Task<VoterRecord?> GetVoterRecordByConfirmationCodeAsync(Guid confirmationCode) =>
            Task.FromResult(RecordToReturn);
    }

    // fake election accessor, dictionary-backed so tests can configure races + candidates per race
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

    private static readonly Guid SampleCode = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [TestMethod]
    public async Task GetByConfirmationCodeAsync_UnknownCode_ReturnsNull()
    {
        var voteAccessor = new FakeVoteAccessor { RecordToReturn = null };
        var manager = new VoteLookupManager(voteAccessor, new FakeElectionAccessor());

        var result = await manager.GetByConfirmationCodeAsync(SampleCode);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByConfirmationCodeAsync_KnownCode_ReturnsDtoWithElectionAndSelections()
    {
        var voteAccessor = new FakeVoteAccessor
        {
            RecordToReturn = new VoterRecord { VoterId = 1, ElectionId = 1, ConfirmationCode = SampleCode },
            VotesToReturn = new List<Vote>
            {
                new() { RaceId = 10, CandidateId = 100 },
                new() { RaceId = 20, CandidateId = 200 }
            }
        };
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = new Election { ElectionId = 1, ElectionName = "2026 General" },
            RacesByElection = new Dictionary<int, List<Race>>
            {
                [1] = new List<Race>
                {
                    new Race { RaceId = 10, ElectionId = 1, RaceName = "Mayor", RaceType = "Candidate" },
                    new Race { RaceId = 20, ElectionId = 1, RaceName = "Proposition A", RaceType = "YesNo" }
                }
            },
            CandidatesByRace = new Dictionary<int, List<Candidate>>
            {
                [10] = new List<Candidate>
                {
                    new Candidate { CandidateId = 100, RaceId = 10, CandidateName = "Lil Red", Party = "Spirit" }
                },
                [20] = new List<Candidate>
                {
                    new Candidate { CandidateId = 200, RaceId = 20, CandidateName = "Yes", Party = null }
                }
            }
        };
        var manager = new VoteLookupManager(voteAccessor, electionAccessor);

        var result = await manager.GetByConfirmationCodeAsync(SampleCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.ElectionId);
        Assert.AreEqual("2026 General", result.ElectionName);
        Assert.AreEqual(2, result.Selections.Count);

        var mayor = result.Selections.First(s => s.RaceId == 10);
        Assert.AreEqual("Mayor", mayor.RaceName);
        Assert.AreEqual("Candidate", mayor.RaceType);
        Assert.AreEqual(100, mayor.CandidateId);
        Assert.AreEqual("Lil Red", mayor.CandidateName);
        Assert.AreEqual("Spirit", mayor.Party);

        var prop = result.Selections.First(s => s.RaceId == 20);
        Assert.AreEqual("Proposition A", prop.RaceName);
        Assert.AreEqual("YesNo", prop.RaceType);
        Assert.AreEqual("Yes", prop.CandidateName);
        Assert.IsNull(prop.Party);
    }

    [TestMethod]
    public async Task GetByConfirmationCodeAsync_RaceDeletedAfterVote_SkipsThatSelection()
    {
        // vote exists for race 99, but race 99 is not in the election's current race list
        var voteAccessor = new FakeVoteAccessor
        {
            RecordToReturn = new VoterRecord { VoterId = 1, ElectionId = 1, ConfirmationCode = SampleCode },
            VotesToReturn = new List<Vote>
            {
                new() { RaceId = 10, CandidateId = 100 },
                new() { RaceId = 99, CandidateId = 999 }  // orphaned
            }
        };
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
        var manager = new VoteLookupManager(voteAccessor, electionAccessor);

        var result = await manager.GetByConfirmationCodeAsync(SampleCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.Selections.Count);
        Assert.AreEqual(10, result.Selections[0].RaceId);
    }

    [TestMethod]
    public async Task GetByConfirmationCodeAsync_UnknownCandidate_ShowsFallbackName()
    {
        // the vote points at a CandidateId the race doesn't know about (data drift)
        var voteAccessor = new FakeVoteAccessor
        {
            RecordToReturn = new VoterRecord { VoterId = 1, ElectionId = 1, ConfirmationCode = SampleCode },
            VotesToReturn = new List<Vote> { new() { RaceId = 10, CandidateId = 404 } }
        };
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
        var manager = new VoteLookupManager(voteAccessor, electionAccessor);

        var result = await manager.GetByConfirmationCodeAsync(SampleCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.Selections.Count);
        Assert.AreEqual("(unknown)", result.Selections[0].CandidateName);
        Assert.IsNull(result.Selections[0].Party);
    }

    [TestMethod]
    public async Task GetByConfirmationCodeAsync_ElectionNoLongerActive_UsesFallbackElectionName()
    {
        // code belongs to election 7, but the active election is a different one (or null)
        var voteAccessor = new FakeVoteAccessor
        {
            RecordToReturn = new VoterRecord { VoterId = 1, ElectionId = 7, ConfirmationCode = SampleCode },
            VotesToReturn = new List<Vote>()
        };
        var electionAccessor = new FakeElectionAccessor
        {
            ActiveElection = null,  // no active election
            RacesByElection = new Dictionary<int, List<Race>>()
        };
        var manager = new VoteLookupManager(voteAccessor, electionAccessor);

        var result = await manager.GetByConfirmationCodeAsync(SampleCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(7, result!.ElectionId);
        Assert.AreEqual("Election 7", result.ElectionName);
    }
}
