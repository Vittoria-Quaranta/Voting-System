namespace VotingSystem.Engines;

/// <summary>
/// Handles password hashing and verification.
/// Pure logic, no database access.
/// </summary>
public interface IAuthEngine
{
    /// <summary>
    /// Check if the plain password matches the stored hash.
    /// Returns false if they don't match or the hash is invalid.
    /// </summary>
    bool VerifyPassword(string plainPassword, string storedHash);

    /// <summary>
    /// Hash a plain password for storage in the database.
    /// </summary>
    string HashPassword(string plainPassword);
}
