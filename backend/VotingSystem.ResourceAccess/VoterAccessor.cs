using Dapper;
using Microsoft.Data.SqlClient;
using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Dapper implementation of IVoterAccessor.
/// Each method opens its own connection and disposes it after. No shared state.
/// </summary>
public class VoterAccessor : IVoterAccessor
{
    private readonly string _connectionString;

    public VoterAccessor(string connectionString)
    {
        _connectionString = connectionString;
    }

    // looks up voter by username for login, returns null if not found
    public async Task<Voter?> GetVoterByUsernameAsync(string username)
    {
        const string sql =
            "SELECT VoterId, FirstName, LastName, Username, PasswordHash, DateOfBirth, RegistrationDate " +
            "FROM Voter WHERE Username = @Username";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Voter>(sql, new { Username = username });
    }

    // checks VoterRecord table to see if this voter already voted in this election
    public async Task<bool> HasVotedInElectionAsync(int voterId, int electionId)
    {
        const string sql =
            "SELECT COUNT(1) FROM VoterRecord " +
            "WHERE VoterId = @VoterId AND ElectionId = @ElectionId";

        using var connection = new SqlConnection(_connectionString);
        var count = await connection.ExecuteScalarAsync<int>(sql, new { VoterId = voterId, ElectionId = electionId });
        return count > 0;
    }
}
