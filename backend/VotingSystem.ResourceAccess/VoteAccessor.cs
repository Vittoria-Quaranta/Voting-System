using Dapper;
using Microsoft.Data.SqlClient;
using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Dapper implementation of IVoteAccessor.
/// SubmitBallotAsync uses a transaction so everything saves or nothing does.
/// </summary>
public class VoteAccessor : IVoteAccessor
{
    private readonly string _connectionString;

    public VoteAccessor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> SubmitBallotAsync(int voterId, int electionId, List<Vote> selections)
    {
        var confirmationCode = Guid.NewGuid();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // record that this voter participated in this election
            const string insertRecord =
                "INSERT INTO VoterRecord (VoterId, ElectionId, ConfirmationCode) " +
                "VALUES (@VoterId, @ElectionId, @ConfirmationCode)";

            await connection.ExecuteAsync(insertRecord, new
            {
                VoterId = voterId,
                ElectionId = electionId,
                ConfirmationCode = confirmationCode
            }, transaction);

            // save each individual race selection
            const string insertVote =
                "INSERT INTO Vote (ConfirmationCode, RaceId, CandidateId) " +
                "VALUES (@ConfirmationCode, @RaceId, @CandidateId)";

            foreach (var selection in selections)
            {
                await connection.ExecuteAsync(insertVote, new
                {
                    ConfirmationCode = confirmationCode,
                    RaceId = selection.RaceId,
                    CandidateId = selection.CandidateId
                }, transaction);
            }

            transaction.Commit();
            return confirmationCode;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // voter uses their confirmation code to verify their ballot was recorded
    public async Task<IEnumerable<Vote>> GetVotesByConfirmationCodeAsync(Guid confirmationCode)
    {
        const string sql =
            "SELECT VoteId, ConfirmationCode, RaceId, CandidateId " +
            "FROM Vote WHERE ConfirmationCode = @ConfirmationCode";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Vote>(sql, new { ConfirmationCode = confirmationCode });
    }

    // resolves a confirmation code back to its VoterRecord (for ElectionId lookup)
    public async Task<VoterRecord?> GetVoterRecordByConfirmationCodeAsync(Guid confirmationCode)
    {
        const string sql =
            "SELECT VoterRecordId, VoterId, ElectionId, ConfirmationCode, SubmittedAt " +
            "FROM VoterRecord WHERE ConfirmationCode = @ConfirmationCode";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<VoterRecord>(sql, new { ConfirmationCode = confirmationCode });
    }

    // count votes per candidate for all races in an election
    public async Task<IEnumerable<VoteCount>> GetVoteCountsByElectionAsync(int electionId)
    {
        const string sql =
            "SELECT v.RaceId, v.CandidateId, COUNT(*) AS [Count] " +
            "FROM Vote v " +
            "INNER JOIN Race r ON v.RaceId = r.RaceId " +
            "WHERE r.ElectionId = @ElectionId " +
            "GROUP BY v.RaceId, v.CandidateId";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<VoteCount>(sql, new { ElectionId = electionId });
    }
}
