using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.DataContracts;
using VotingSystem.Engines;

namespace VotingSystem.Tests;

[TestClass]
public class BallotValidationEngineTests
{
    private BallotValidationEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new BallotValidationEngine();
    }

    // small helper to build a test ballot
    private static BallotDto MakeBallot() => new()
    {
        ElectionId = 1,
        ElectionName = "Test",
        Races = new List<RaceDto>
        {
            new RaceDto
            {
                RaceId = 10, RaceName = "Mayor", RaceType = "Candidate",
                Choices = new List<ChoiceDto>
                {
                    new ChoiceDto { ChoiceId = 100, ChoiceName = "Lil Red" },
                    new ChoiceDto { ChoiceId = 101, ChoiceName = "Herbie" }
                }
            },
            new RaceDto
            {
                RaceId = 20, RaceName = "Prop 1", RaceType = "YesNo",
                Choices = new List<ChoiceDto>
                {
                    new ChoiceDto { ChoiceId = 200, ChoiceName = "Yes" },
                    new ChoiceDto { ChoiceId = 201, ChoiceName = "No" }
                }
            }
        }
    };

    [TestMethod]
    public void Validate_AllRacesCovered_ReturnsValid()
    {
        var selections = new List<SelectionDto>
        {
            new SelectionDto { RaceId = 10, ChoiceId = 100 },
            new SelectionDto { RaceId = 20, ChoiceId = 201 }
        };

        var result = _engine.Validate(MakeBallot(), selections);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_DuplicateRaceSelection_ReturnsInvalid()
    {
        var selections = new List<SelectionDto>
        {
            new SelectionDto { RaceId = 10, ChoiceId = 100 },
            new SelectionDto { RaceId = 10, ChoiceId = 101 },
            new SelectionDto { RaceId = 20, ChoiceId = 200 }
        };

        var result = _engine.Validate(MakeBallot(), selections);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.ErrorMessage.Contains("Duplicate"));
    }

    [TestMethod]
    public void Validate_MissingRaceSelection_ReturnsInvalid()
    {
        // only selecting mayor, missing prop 1
        var selections = new List<SelectionDto>
        {
            new SelectionDto { RaceId = 10, ChoiceId = 100 }
        };

        var result = _engine.Validate(MakeBallot(), selections);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.ErrorMessage.Contains("Missing"));
    }

    [TestMethod]
    public void Validate_UnknownRaceId_ReturnsInvalid()
    {
        var selections = new List<SelectionDto>
        {
            new SelectionDto { RaceId = 10, ChoiceId = 100 },
            new SelectionDto { RaceId = 20, ChoiceId = 200 },
            new SelectionDto { RaceId = 999, ChoiceId = 1 }  // race not on ballot
        };

        var result = _engine.Validate(MakeBallot(), selections);

        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void Validate_ChoiceNotInRace_ReturnsInvalid()
    {
        var selections = new List<SelectionDto>
        {
            new SelectionDto { RaceId = 10, ChoiceId = 200 },  // "Yes" isn't a mayor choice
            new SelectionDto { RaceId = 20, ChoiceId = 201 }
        };

        var result = _engine.Validate(MakeBallot(), selections);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.ErrorMessage.Contains("unknown choice"));
    }

    [TestMethod]
    public void Validate_EmptySelections_ReturnsInvalid()
    {
        var result = _engine.Validate(MakeBallot(), new List<SelectionDto>());

        Assert.IsFalse(result.IsValid);
    }
}
