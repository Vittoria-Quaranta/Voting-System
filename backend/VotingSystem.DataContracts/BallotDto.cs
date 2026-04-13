namespace VotingSystem.DataContracts;

/// <summary>
/// The full ballot the frontend renders for a voter.
/// Contains the election info and all races nested inside.
/// </summary>
public class BallotDto
{
    public int ElectionId { get; set; }
    public string ElectionName { get; set; } = string.Empty;
    public List<RaceDto> Races { get; set; } = new();
}

/// <summary>
/// A single race within the ballot with all its choices.
/// </summary>
public class RaceDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public string RaceType { get; set; } = string.Empty;
    public List<ChoiceDto> Choices { get; set; } = new();
}

/// <summary>
/// A single choice in a race (candidate for an office or Yes/No for a proposition).
/// </summary>
public class ChoiceDto
{
    public int ChoiceId { get; set; }
    public string ChoiceName { get; set; } = string.Empty;
    public string? Party { get; set; }
}
