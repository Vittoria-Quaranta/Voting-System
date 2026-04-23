namespace VotingSystem.DataContracts;

/// <summary>
/// Returned by GET /api/vote-lookup/{code}.
/// Shows the voter what they actually picked, so they can verify their ballot was recorded correctly.
/// </summary>
public class VoteLookupDto
{
    public int ElectionId { get; set; }
    public string ElectionName { get; set; } = string.Empty;
    public List<VoteLookupItemDto> Selections { get; set; } = new();
}

/// <summary>
/// One race + the candidate this ballot picked for it.
/// </summary>
public class VoteLookupItemDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public string RaceType { get; set; } = string.Empty;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string? Party { get; set; }
}
