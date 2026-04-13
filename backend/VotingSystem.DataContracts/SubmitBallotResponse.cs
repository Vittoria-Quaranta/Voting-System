namespace VotingSystem.DataContracts;

/// <summary>
/// Backend response after a ballot submit attempt.
/// ConfirmationCode is set on success so the voter can verify their ballot later.
/// </summary>
public class SubmitBallotResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ConfirmationCode { get; set; }
}
