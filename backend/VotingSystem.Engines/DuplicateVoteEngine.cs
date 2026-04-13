using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Enforces the "one vote per election" rule.
/// The manager fetches the hasAlreadyVoted flag from the accessor and passes it here.
/// </summary>
public class DuplicateVoteEngine : IDuplicateVoteEngine
{
    public ValidationResult Check(bool hasAlreadyVoted)
    {
        if (hasAlreadyVoted)
        {
            return ValidationResult.Invalid("You have already voted in this election.");
        }

        return ValidationResult.Valid();
    }
}
