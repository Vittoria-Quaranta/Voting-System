using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.Engines;

namespace VotingSystem.Tests;

[TestClass]
public class AuthEngineTests
{
    private AuthEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new AuthEngine();
    }

    [TestMethod]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var hash = _engine.HashPassword("password123");
        Assert.IsFalse(string.IsNullOrEmpty(hash));
    }

    [TestMethod]
    public void HashPassword_DifferentEachTime()
    {
        // BCrypt uses a random salt so the same password makes different hashes
        var hash1 = _engine.HashPassword("password123");
        var hash2 = _engine.HashPassword("password123");
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var hash = _engine.HashPassword("password123");
        var result = _engine.VerifyPassword("password123", hash);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = _engine.HashPassword("password123");
        var result = _engine.VerifyPassword("wrongpassword", hash);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void VerifyPassword_EmptyPlainPassword_ReturnsFalse()
    {
        var hash = _engine.HashPassword("password123");
        var result = _engine.VerifyPassword("", hash);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void VerifyPassword_EmptyHash_ReturnsFalse()
    {
        var result = _engine.VerifyPassword("password123", "");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void VerifyPassword_MalformedHash_ReturnsFalse()
    {
        // bad hash should fail closed, not throw
        var result = _engine.VerifyPassword("password123", "not_a_real_hash");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void VerifyPassword_CaseSensitive()
    {
        var hash = _engine.HashPassword("Password123");
        var result = _engine.VerifyPassword("password123", hash);
        Assert.IsFalse(result);
    }
}
