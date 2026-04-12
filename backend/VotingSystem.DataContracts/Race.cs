namespace VotingSystem.DataContracts;

/// <summary>
/// A single race or ballot issue within an election.
/// RaceType is either "Candidate" or "YesNo".
/// </summary>
public class Race
{
    public int RaceId { get; set; }
    public int ElectionId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public string RaceType { get; set; } = "Candidate";
}
