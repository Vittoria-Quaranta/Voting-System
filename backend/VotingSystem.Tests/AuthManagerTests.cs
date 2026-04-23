using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class AuthManagerTests
{
    // hand-rolled fake voter accessor, returns whatever we tell it to
    private class FakeVoterAccessor : IVoterAccessor
    {
        public Voter? VoterToReturn { get; set; }

        public Task<Voter?> GetVoterByUsernameAsync(string username) => Task.FromResult(VoterToReturn);

        public Task<bool> HasVotedInElectionAsync(int voterId, int electionId) => Task.FromResult(false);

        public Task<int> CreateVoterAsync(Voter voter) => Task.FromResult(0);
    }

    // fake election accessor, returns null (no active election) by default
    private class FakeElectionAccessor : IElectionAccessor
    {
        public Election? ElectionToReturn { get; set; }
        public Task<Election?> GetActiveElectionAsync() => Task.FromResult(ElectionToReturn);
        public Task<IEnumerable<Race>> GetRacesByElectionAsync(int electionId) =>
            Task.FromResult<IEnumerable<Race>>(new List<Race>());
        public Task<IEnumerable<Candidate>> GetCandidatesByRaceAsync(int raceId) =>
            Task.FromResult<IEnumerable<Candidate>>(new List<Candidate>());
    }

    // fake auth engine, returns whatever we tell it to for verify
    private class FakeAuthEngine : IAuthEngine
    {
        public bool VerifyResult { get; set; }

        public bool VerifyPassword(string plainPassword, string storedHash) => VerifyResult;

        public string HashPassword(string plainPassword) => "fake_hash";
    }

    [TestMethod]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        var fakeAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter
            {
                VoterId = 1,
                FirstName = "Tommie",
                LastName = "Frazier",
                Username = "tfrazier",
                PasswordHash = "some_hash"
            }
        };
        var fakeEngine = new FakeAuthEngine { VerifyResult = true };
        var manager = new AuthManager(fakeAccessor, new FakeElectionAccessor(), fakeEngine);

        var response = await manager.LoginAsync(new LoginRequest { Username = "tfrazier", Password = "correct" });

        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.VoterId);
        Assert.AreEqual("Tommie", response.FirstName);
    }

    [TestMethod]
    public async Task LoginAsync_UsernameNotFound_ReturnsFailure()
    {
        var fakeAccessor = new FakeVoterAccessor { VoterToReturn = null };
        var fakeEngine = new FakeAuthEngine { VerifyResult = true };
        var manager = new AuthManager(fakeAccessor, new FakeElectionAccessor(), fakeEngine);

        var response = await manager.LoginAsync(new LoginRequest { Username = "nobody", Password = "whatever" });

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Invalid username or password.", response.Message);
    }

    [TestMethod]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        var fakeAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier", PasswordHash = "some_hash" }
        };
        var fakeEngine = new FakeAuthEngine { VerifyResult = false };
        var manager = new AuthManager(fakeAccessor, new FakeElectionAccessor(), fakeEngine);

        var response = await manager.LoginAsync(new LoginRequest { Username = "tfrazier", Password = "wrong" });

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Invalid username or password.", response.Message);
    }

    [TestMethod]
    public async Task LoginAsync_EmptyUsername_ReturnsFailure()
    {
        var fakeAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier", PasswordHash = "h" }
        };
        var manager = new AuthManager(fakeAccessor, new FakeElectionAccessor(), new FakeAuthEngine { VerifyResult = true });

        var response = await manager.LoginAsync(new LoginRequest { Username = "", Password = "password" });

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Invalid username or password.", response.Message);
    }

    [TestMethod]
    public async Task LoginAsync_EmptyPassword_ReturnsFailure()
    {
        var fakeAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier", PasswordHash = "h" }
        };
        var manager = new AuthManager(fakeAccessor, new FakeElectionAccessor(), new FakeAuthEngine { VerifyResult = true });

        var response = await manager.LoginAsync(new LoginRequest { Username = "tfrazier", Password = "" });

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Invalid username or password.", response.Message);
    }

    [TestMethod]
    public async Task LoginAsync_BothFailureCases_UseSameMessage()
    {
        // security check: don't leak whether the username or password was wrong
        var noUserAccessor = new FakeVoterAccessor { VoterToReturn = null };
        var wrongPassAccessor = new FakeVoterAccessor
        {
            VoterToReturn = new Voter { VoterId = 1, Username = "tfrazier", PasswordHash = "h" }
        };

        var manager1 = new AuthManager(noUserAccessor, new FakeElectionAccessor(), new FakeAuthEngine { VerifyResult = true });
        var manager2 = new AuthManager(wrongPassAccessor, new FakeElectionAccessor(), new FakeAuthEngine { VerifyResult = false });

        var resp1 = await manager1.LoginAsync(new LoginRequest { Username = "a", Password = "b" });
        var resp2 = await manager2.LoginAsync(new LoginRequest { Username = "a", Password = "b" });

        Assert.AreEqual(resp1.Message, resp2.Message);
    }
}
