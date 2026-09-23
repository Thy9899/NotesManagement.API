using Dapper;
using NotesManagement.API.Data;
using NotesManagement.API.Models;

namespace NotesManagement.API.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public NoteRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /*
    |-------------------------------------------------------------------
    | GET ALL NOTES
    |-------------------------------------------------------------------
    */
    public async Task<IEnumerable<Note>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                UserId,
                Title,
                Content,
                CreatedAt,
                UpdatedAt
            FROM Notes
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC;
            """;

        return await connection.QueryAsync<Note>(
            sql,
            new { UserId = userId }
        );
    }

    /*
    |-------------------------------------------------------------------
    | GET NOTE BY ID
    |-------------------------------------------------------------------
    */
    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                UserId,
                Title,
                Content,
                CreatedAt,
                UpdatedAt
            FROM Notes
            WHERE Id = @Id
              AND UserId = @UserId;
            """;

        return await connection.QuerySingleOrDefaultAsync<Note>(
            sql,
            new
            {
                Id = id,
                UserId = userId
            }
        );
    }

    /*
    |-------------------------------------------------------------------
    | CREATE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<int> CreateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Notes
            (
                UserId,
                Title,
                Content
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @UserId,
                @Title,
                @Content
            );
            """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            note
        );
    }

    /*
    |-------------------------------------------------------------------
    | UPDATE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<bool> UpdateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE Notes
            SET
                Title = @Title,
                Content = @Content,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id
              AND UserId = @UserId;
            """;

        var affectedRows = await connection.ExecuteAsync(
            sql,
            note
        );

        return affectedRows > 0;
    }

    /*
    |-------------------------------------------------------------------
    | DELETE NOTE
    |-------------------------------------------------------------------
    */
    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM Notes
            WHERE Id = @Id
              AND UserId = @UserId;
            """;

        var affectedRows = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                UserId = userId
            }
        );

        return affectedRows > 0;
    }
}
