using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Checks whether a voter is allowed to vote based on their prior voting status.
/// Pure logic, no database access.
/// </summary>
public interface IDuplicateVoteEngine
{
    /// <summary>
    /// Returns valid if the voter has not voted yet, invalid if they have.
    /// </summary>
    ValidationResult Check(bool hasAlreadyVoted);
}
