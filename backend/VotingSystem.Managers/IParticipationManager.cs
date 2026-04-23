using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Answers the "did voter X participate in the active election" question
/// for third-party observers, without revealing any ballot choices.
/// </summary>
public interface IParticipationManager
{
    /// <summary>
    /// Returns whether the voter with the given username has voted in the active election.
    /// Unknown usernames and "no active election" both return Voted = false.
    /// </summary>
    Task<ParticipationDto> CheckAsync(string username);
}
