namespace VotingSystem.DataContracts;

/// <summary>
/// A candidate or option within a race.
/// For yes/no issues, there are two candidates: "Yes" and "No".
/// </summary>
public class Candidate
{
    public int CandidateId { get; set; }
    public int RaceId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string? Party { get; set; }
}
