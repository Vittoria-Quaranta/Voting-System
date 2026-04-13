namespace VotingSystem.DataContracts;

/// <summary>
/// A single choice the voter made for one race.
/// Used during review and submit.
/// </summary>
public class SelectionDto
{
    public int RaceId { get; set; }
    public int ChoiceId { get; set; }
}
