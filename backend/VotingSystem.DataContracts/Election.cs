namespace VotingSystem.DataContracts;

/// <summary>
/// An election event (e.g. "2026 Pacopolis General Election").
/// Contains one or more races.
/// </summary>
public class Election
{
    public int ElectionId { get; set; }
    public string ElectionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
