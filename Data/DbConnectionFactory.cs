using Microsoft.Data.SqlClient;
using System.Data;

namespace NotesManagement.API.Data;

public class DbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /*
    |-------------------------------------------------------------------
    | CREATES AND RETURN A NEW DATABASE CONNECTION
    |-------------------------------------------------------------------
    */
    public IDbConnection CreateConnection()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' was not found."
            );
        }

        return new SqlConnection(connectionString);
    }
}