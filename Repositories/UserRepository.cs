using Dapper;
using NotesManagement.API.Data;
using NotesManagement.API.Models;

namespace NotesManagement.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /*
    |-------------------------------------------------------------------
    | FIND USER BY ID
    |-------------------------------------------------------------------
    */
    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                Email,
                PasswordHash,
                CreatedAt
            FROM Users
            WHERE Id = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { Id = id }
        );
    }

    /*
    |-------------------------------------------------------------------
    | FIND USER BY EMAIL
    |-------------------------------------------------------------------
    */
    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                Email,
                PasswordHash,
                CreatedAt
            FROM Users
            WHERE Email = @Email;
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { Email = email }
        );
    }

    /*
    |-------------------------------------------------------------------
    | CREATE USER [REGISTER]
    |-------------------------------------------------------------------
    */
    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Users
            (
                Name,
                Email,
                PasswordHash
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Name,
                @Email,
                @PasswordHash
            );
            """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            user
        );
    }
}