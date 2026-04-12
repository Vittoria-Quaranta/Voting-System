using Dapper;
using Microsoft.Data.SqlClient;
using VotingSystem.DataContracts;

namespace VotingSystem.ResourceAccess;

/// <summary>
/// Dapper implementation of IElectionAccessor.
/// Each method opens its own connection and disposes it after.
/// </summary>
public class ElectionAccessor : IElectionAccessor
{
    private readonly string _connectionString;

    public ElectionAccessor(string connectionString)
    {
        _connectionString = connectionString;
    }

    // grabs the election where IsActive = 1, null if nothing is active
    public async Task<Election?> GetActiveElectionAsync()
    {
        const string sql =
            "SELECT ElectionId, ElectionName, StartDate, EndDate, IsActive " +
            "FROM Election WHERE IsActive = 1";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Election>(sql);
    }

    // all races for an election (mayor, city council, propositions, etc.)
    public async Task<IEnumerable<Race>> GetRacesByElectionAsync(int electionId)
    {
        const string sql =
            "SELECT RaceId, ElectionId, RaceName, RaceType " +
            "FROM Race WHERE ElectionId = @ElectionId";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Race>(sql, new { ElectionId = electionId });
    }

    // all candidates/options for a race (or Yes/No for propositions)
    public async Task<IEnumerable<Candidate>> GetCandidatesByRaceAsync(int raceId)
    {
        const string sql =
            "SELECT CandidateId, RaceId, CandidateName, Party " +
            "FROM Candidate WHERE RaceId = @RaceId";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Candidate>(sql, new { RaceId = raceId });
    }
}
