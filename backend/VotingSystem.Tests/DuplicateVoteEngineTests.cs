using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.Engines;

namespace VotingSystem.Tests;

[TestClass]
public class DuplicateVoteEngineTests
{
    private DuplicateVoteEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new DuplicateVoteEngine();
    }

    [TestMethod]
    public void Check_VoterHasNotVoted_ReturnsValid()
    {
        // first vote should be allowed
        var result = _engine.Check(hasAlreadyVoted: false);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Check_VoterAlreadyVoted_ReturnsInvalid()
    {
        // second vote should be blocked
        var result = _engine.Check(hasAlreadyVoted: true);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.ErrorMessage.Contains("already voted"));
    }

    [TestMethod]
    public void Check_VoterAlreadyVoted_MessageIsClear()
    {
        var result = _engine.Check(hasAlreadyVoted: true);

        Assert.AreEqual("You have already voted in this election.", result.ErrorMessage);
    }
}
