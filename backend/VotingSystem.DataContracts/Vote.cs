namespace VotingSystem.DataContracts;

/// <summary>
/// A single ballot selection (one per race).
/// Linked to the voter only through ConfirmationCode, not VoterId,
/// so you can't trace a vote back to a person without the code.
/// </summary>
public class Vote
{
    public int VoteId { get; set; }
    public Guid ConfirmationCode { get; set; }
    public int RaceId { get; set; }
    public int CandidateId { get; set; }
}
