namespace VotingSystem.DataContracts;

/// <summary>
/// What the frontend sends when a voter tries to log in.
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
