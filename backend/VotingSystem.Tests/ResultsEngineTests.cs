using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;

namespace VotingSystem.Tests;

[TestClass]
public class ResultsEngineTests
{
    private ResultsEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new ResultsEngine();
    }

    [TestMethod]
    public void BuildRaceResult_PicksWinnerWithMostVotes()
    {
        var race = new Race { RaceId = 1, RaceName = "Mayor", RaceType = "Candidate" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Alice", Party = "Dem" },
            new() { CandidateId = 2, RaceId = 1, CandidateName = "Bob", Party = "Rep" },
        };
        var counts = new List<VoteCount>
        {
            new() { RaceId = 1, CandidateId = 1, Count = 100 },
            new() { RaceId = 1, CandidateId = 2, Count = 75 },
        };

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.AreEqual(175, result.TotalVotes);
        Assert.AreEqual(2, result.Candidates.Count);
        Assert.IsTrue(result.Candidates[0].IsWinner);
        Assert.AreEqual("Alice", result.Candidates[0].CandidateName);
        Assert.IsFalse(result.Candidates[1].IsWinner);
    }

    [TestMethod]
    public void BuildRaceResult_TieMarksBothAsWinners()
    {
        var race = new Race { RaceId = 1, RaceName = "Council", RaceType = "Candidate" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Alice" },
            new() { CandidateId = 2, RaceId = 1, CandidateName = "Bob" },
        };
        var counts = new List<VoteCount>
        {
            new() { RaceId = 1, CandidateId = 1, Count = 50 },
            new() { RaceId = 1, CandidateId = 2, Count = 50 },
        };

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.IsTrue(result.Candidates[0].IsWinner);
        Assert.IsTrue(result.Candidates[1].IsWinner);
    }

    [TestMethod]
    public void BuildRaceResult_NoVotesShowsZeroPercent()
    {
        var race = new Race { RaceId = 1, RaceName = "Mayor", RaceType = "Candidate" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Alice" },
        };
        var counts = new List<VoteCount>();

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.AreEqual(0, result.TotalVotes);
        Assert.AreEqual(0, result.Candidates[0].Percentage);
        Assert.IsFalse(result.Candidates[0].IsWinner);
    }

    [TestMethod]
    public void BuildRaceResult_CalculatesPercentagesCorrectly()
    {
        var race = new Race { RaceId = 1, RaceName = "Prop A", RaceType = "YesNo" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Yes" },
            new() { CandidateId = 2, RaceId = 1, CandidateName = "No" },
        };
        var counts = new List<VoteCount>
        {
            new() { RaceId = 1, CandidateId = 1, Count = 3 },
            new() { RaceId = 1, CandidateId = 2, Count = 1 },
        };

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.AreEqual(75.0, result.Candidates[0].Percentage);
        Assert.AreEqual(25.0, result.Candidates[1].Percentage);
        Assert.IsTrue(result.Candidates[0].IsWinner); // Yes wins
    }

    [TestMethod]
    public void BuildRaceResult_SortsCandidatesByVotesDescending()
    {
        var race = new Race { RaceId = 1, RaceName = "Council", RaceType = "Candidate" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Last" },
            new() { CandidateId = 2, RaceId = 1, CandidateName = "First" },
            new() { CandidateId = 3, RaceId = 1, CandidateName = "Middle" },
        };
        var counts = new List<VoteCount>
        {
            new() { RaceId = 1, CandidateId = 1, Count = 10 },
            new() { RaceId = 1, CandidateId = 2, Count = 100 },
            new() { RaceId = 1, CandidateId = 3, Count = 50 },
        };

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.AreEqual("First", result.Candidates[0].CandidateName);
        Assert.AreEqual("Middle", result.Candidates[1].CandidateName);
        Assert.AreEqual("Last", result.Candidates[2].CandidateName);
    }

    [TestMethod]
    public void BuildRaceResult_IgnoresCountsFromOtherRaces()
    {
        var race = new Race { RaceId = 1, RaceName = "Mayor", RaceType = "Candidate" };
        var candidates = new List<Candidate>
        {
            new() { CandidateId = 1, RaceId = 1, CandidateName = "Alice" },
        };
        var counts = new List<VoteCount>
        {
            new() { RaceId = 1, CandidateId = 1, Count = 10 },
            new() { RaceId = 99, CandidateId = 99, Count = 500 }, // different race
        };

        var result = _engine.BuildRaceResult(race, candidates, counts);

        Assert.AreEqual(10, result.TotalVotes);
    }
}
