namespace VotingSystem.DataContracts;

/// <summary>
/// What the frontend sends when a voter submits their finished ballot.
/// </summary>
public class SubmitBallotRequest
{
    public int VoterId { get; set; }
    public int ElectionId { get; set; }
    public List<SelectionDto> Selections { get; set; } = new();
}
