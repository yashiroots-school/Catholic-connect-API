using System.Data;
using Microsoft.Data.SqlClient;

namespace ChurchAPI.Services
{
    public class DbLogger
    {
        private readonly string _connectionString;

        public DbLogger(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task LogApiCallAsync(string procName, string parameters, string response, string? userId = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("InsertApiLog", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProcName", procName);
                command.Parameters.AddWithValue("@Parameters", parameters ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Response", response ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserId", userId ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
