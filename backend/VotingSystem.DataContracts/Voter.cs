namespace VotingSystem.DataContracts;

/// <summary>
/// Represents a registered voter in the system.
/// </summary>
public class Voter
{
    public int VoterId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegistrationDate { get; set; }
}
