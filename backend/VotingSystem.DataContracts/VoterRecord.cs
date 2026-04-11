namespace VotingSystem.DataContracts;

/// <summary>
/// Tracks that a voter participated in an election.
/// Does NOT store their actual picks — that's in Vote.
/// The ConfirmationCode lets the voter look up their ballot later.
/// </summary>
public class VoterRecord
{
    public int VoterRecordId { get; set; }
    public int VoterId { get; set; }
    public int ElectionId { get; set; }
    public Guid ConfirmationCode { get; set; }
    public DateTime SubmittedAt { get; set; }
}
