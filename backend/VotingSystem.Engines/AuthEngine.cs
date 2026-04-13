namespace VotingSystem.Engines;

/// <summary>
/// BCrypt-based auth engine. Uses BCrypt.Net-Next for password operations.
/// </summary>
public class AuthEngine : IAuthEngine
{
    // checks a plain password against a stored BCrypt hash
    public bool VerifyPassword(string plainPassword, string storedHash)
    {
        if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
        }
        catch
        {
            // bad hash format, missing salt, etc. fail closed instead of crashing
            return false;
        }
    }

    // creates a salted BCrypt hash ready to save in the Voter table
    public string HashPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }
}
