namespace VotingSystem.DataContracts;

/// <summary>
/// What the backend sends back after a registration attempt.
/// Mirrors LoginResponse (minus HasVoted) so the frontend can treat
/// a successful registration the same way it treats a successful login.
/// </summary>
public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? VoterId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
