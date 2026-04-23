namespace VotingSystem.DataContracts;

/// <summary>
/// Full election results returned by GET /api/results.
/// </summary>
public class ResultsDto
{
    public int ElectionId { get; set; }
    public string ElectionName { get; set; } = string.Empty;
    public List<RaceResultDto> Races { get; set; } = new();
}

/// <summary>
/// Results for a single race, including each candidate's vote count and who won.
/// </summary>
public class RaceResultDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public string RaceType { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public List<CandidateResultDto> Candidates { get; set; } = new();
}

/// <summary>
/// A single candidate's result within a race.
/// </summary>
public class CandidateResultDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string? Party { get; set; }
    public int Votes { get; set; }
    public double Percentage { get; set; }
    public bool IsWinner { get; set; }
}

/// <summary>
/// Raw vote count from the database, one row per candidate.
/// </summary>
public class VoteCount
{
    public int RaceId { get; set; }
    public int CandidateId { get; set; }
    public int Count { get; set; }
}
