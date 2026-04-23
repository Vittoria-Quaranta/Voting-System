namespace VotingSystem.DataContracts;

/// <summary>
/// Returned by GET /api/participation?username=...
/// Only says whether the voter participated in the active election; never reveals choices.
/// Unknown usernames return Voted = false (same as "has not voted").
/// </summary>
public class ParticipationDto
{
    public bool Voted { get; set; }
}
