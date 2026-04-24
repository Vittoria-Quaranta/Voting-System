using VotingSystem.DataContracts;

namespace VotingSystem.Managers;

/// <summary>
/// Looks up a submitted ballot by confirmation code so the voter can verify their selections.
/// </summary>
public interface IVoteLookupManager
{
    /// <summary>
    /// Returns the selections for the given confirmation code, or null if the code is not found.
    /// </summary>
    Task<VoteLookupDto?> GetByConfirmationCodeAsync(Guid confirmationCode);
}
