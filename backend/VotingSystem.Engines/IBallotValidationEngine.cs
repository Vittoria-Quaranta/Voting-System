using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Checks that a set of selections is valid against the current ballot.
/// Pure logic, no database access.
/// </summary>
public interface IBallotValidationEngine
{
    ValidationResult Validate(BallotDto ballot, List<SelectionDto> selections);
}
