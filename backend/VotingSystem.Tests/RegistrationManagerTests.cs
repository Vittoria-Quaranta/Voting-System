using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;
using VotingSystem.Managers;
using VotingSystem.ResourceAccess;

namespace VotingSystem.Tests;

[TestClass]
public class RegistrationManagerTests
{
    // fake voter accessor that captures whatever voter gets inserted
    private class FakeVoterAccessor : IVoterAccessor
    {
        public Voter? ExistingVoter { get; set; }
        public int NextVoterId { get; set; } = 42;
        public Voter? CapturedInsert { get; set; }
        public int CreateCallCount { get; set; }

        public Task<Voter?> GetVoterByUsernameAsync(string username) => Task.FromResult(ExistingVoter);
        public Task<bool> HasVotedInElectionAsync(int voterId, int electionId) => Task.FromResult(false);

        public Task<int> CreateVoterAsync(Voter voter)
        {
            CapturedInsert = voter;
            CreateCallCount++;
            return Task.FromResult(NextVoterId);
        }
    }

    private class FakeAuthEngine : IAuthEngine
    {
        public bool VerifyPassword(string plainPassword, string storedHash) => true;
        public string HashPassword(string plainPassword) => $"hashed:{plainPassword}";
    }

    private static RegisterRequest ValidRequest() => new()
    {
        FirstName = "Test",
        LastName = "User",
        Username = "testuser",
        Password = "husker2026",
        DateOfBirth = new DateTime(2000, 1, 1)
    };

    [TestMethod]
    public async Task RegisterAsync_EmptyFirstName_ReturnsFailure()
    {
        var manager = new RegistrationManager(new FakeVoterAccessor(), new FakeAuthEngine());
        var request = ValidRequest();
        request.FirstName = "";

        var response = await manager.RegisterAsync(request);

        Assert.IsFalse(response.Success);
        StringAssert.Contains(response.Message, "First name");
    }

    [TestMethod]
    public async Task RegisterAsync_EmptyLastName_ReturnsFailure()
    {
        var manager = new RegistrationManager(new FakeVoterAccessor(), new FakeAuthEngine());
        var request = ValidRequest();
        request.LastName = "   ";

        var response = await manager.RegisterAsync(request);

        Assert.IsFalse(response.Success);
        StringAssert.Contains(response.Message, "Last name");
    }

    [TestMethod]
    public async Task RegisterAsync_EmptyUsername_ReturnsFailure()
    {
        var manager = new RegistrationManager(new FakeVoterAccessor(), new FakeAuthEngine());
        var request = ValidRequest();
        request.Username = "";

        var response = await manager.RegisterAsync(request);

        Assert.IsFalse(response.Success);
        StringAssert.Contains(response.Message, "Username");
    }

    [TestMethod]
    public async Task RegisterAsync_EmptyPassword_ReturnsFailure()
    {
        var manager = new RegistrationManager(new FakeVoterAccessor(), new FakeAuthEngine());
        var request = ValidRequest();
        request.Password = "";

        var response = await manager.RegisterAsync(request);

        Assert.IsFalse(response.Success);
        StringAssert.Contains(response.Message, "Password");
    }

    [TestMethod]
    public async Task RegisterAsync_PasswordTooShort_ReturnsFailure()
    {
        var manager = new RegistrationManager(new FakeVoterAccessor(), new FakeAuthEngine());
        var request = ValidRequest();
        request.Password = "short12";  // 7 chars

        var response = await manager.RegisterAsync(request);

        Assert.IsFalse(response.Success);
        StringAssert.Contains(response.Message, "8 characters");
    }

    [TestMethod]
    public async Task RegisterAsync_UsernameAlreadyExists_ReturnsFailure()
    {
        var accessor = new FakeVoterAccessor
        {
            ExistingVoter = new Voter { VoterId = 1, Username = "testuser" }
        };
        var manager = new RegistrationManager(accessor, new FakeAuthEngine());

        var response = await manager.RegisterAsync(ValidRequest());

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Username already taken.", response.Message);
        Assert.AreEqual(0, accessor.CreateCallCount);  // never attempts the insert
    }

    [TestMethod]
    public async Task RegisterAsync_ValidInput_CreatesVoterAndReturnsSuccess()
    {
        var accessor = new FakeVoterAccessor { NextVoterId = 99 };
        var manager = new RegistrationManager(accessor, new FakeAuthEngine());

        var response = await manager.RegisterAsync(ValidRequest());

        Assert.IsTrue(response.Success);
        Assert.AreEqual(99, response.VoterId);
        Assert.AreEqual("Test", response.FirstName);
        Assert.AreEqual("User", response.LastName);

        Assert.AreEqual(1, accessor.CreateCallCount);
        Assert.IsNotNull(accessor.CapturedInsert);
        Assert.AreEqual("testuser", accessor.CapturedInsert!.Username);
        // password was hashed, not stored as plaintext
        Assert.AreNotEqual("husker2026", accessor.CapturedInsert.PasswordHash);
        Assert.AreEqual("hashed:husker2026", accessor.CapturedInsert.PasswordHash);
    }

    [TestMethod]
    public async Task RegisterAsync_TrimsWhitespaceFromNameAndUsername()
    {
        var accessor = new FakeVoterAccessor();
        var manager = new RegistrationManager(accessor, new FakeAuthEngine());
        var request = ValidRequest();
        request.FirstName = "  Test  ";
        request.LastName = " User ";
        request.Username = "  testuser  ";

        var response = await manager.RegisterAsync(request);

        Assert.IsTrue(response.Success);
        Assert.AreEqual("Test", accessor.CapturedInsert!.FirstName);
        Assert.AreEqual("User", accessor.CapturedInsert.LastName);
        Assert.AreEqual("testuser", accessor.CapturedInsert.Username);
    }
}
