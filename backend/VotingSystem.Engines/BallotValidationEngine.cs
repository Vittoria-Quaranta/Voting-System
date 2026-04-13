using VotingSystem.DataContracts;

namespace VotingSystem.Engines;

/// <summary>
/// Validates a voter's selections against the ballot they were given.
/// </summary>
public class BallotValidationEngine : IBallotValidationEngine
{
    public ValidationResult Validate(BallotDto ballot, List<SelectionDto> selections)
    {
        // rule 1: must have at least one selection
        if (selections == null || selections.Count == 0)
        {
            return ValidationResult.Invalid("No selections were submitted.");
        }

        // rule 2: no duplicate races (can't pick two mayors)
        var raceIds = selections.Select(s => s.RaceId).ToList();
        if (raceIds.Count != raceIds.Distinct().Count())
        {
            return ValidationResult.Invalid("Duplicate selection for a race.");
        }

        // rule 3: every race on the ballot must have a selection
        foreach (var race in ballot.Races)
        {
            if (!raceIds.Contains(race.RaceId))
            {
                return ValidationResult.Invalid($"Missing selection for race: {race.RaceName}.");
            }
        }

        // rule 4 and 5: every selection must match a real race and a real choice in that race
        foreach (var selection in selections)
        {
            var race = ballot.Races.FirstOrDefault(r => r.RaceId == selection.RaceId);
            if (race == null)
            {
                return ValidationResult.Invalid($"Selection for unknown race: {selection.RaceId}.");
            }

            if (!race.Choices.Any(c => c.ChoiceId == selection.ChoiceId))
            {
                return ValidationResult.Invalid($"Selection for unknown choice in race: {race.RaceName}.");
            }
        }

        return ValidationResult.Valid();
    }
}
