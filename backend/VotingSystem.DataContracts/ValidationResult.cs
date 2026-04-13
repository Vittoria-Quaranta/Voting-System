namespace VotingSystem.DataContracts;

/// <summary>
/// Generic validation outcome used by engines and managers.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ValidationResult Valid() => new() { IsValid = true };
    public static ValidationResult Invalid(string message) => new() { IsValid = false, ErrorMessage = message };
}
