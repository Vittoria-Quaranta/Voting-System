namespace VotingSystem.DataContracts;

/// <summary>
/// What the backend sends back after a login attempt.
/// </summary>
public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? VoterId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
