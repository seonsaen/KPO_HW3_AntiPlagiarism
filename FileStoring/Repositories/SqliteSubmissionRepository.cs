using FileStoring.Entities.Models;
using Microsoft.Data.Sqlite;

namespace FileStoring.Repositories;

public class SqliteSubmissionRepository : ISubmissionRepository
{
    private readonly string _dbPath;
    public SqliteSubmissionRepository(IConfiguration config)
    {
        var dataDir = config.GetValue<string>("Storage:DataDir") ?? "/app/data";
        Directory.CreateDirectory(dataDir);
        _dbPath = Path.Combine(dataDir, "filestoring.db");
        EnsureDb();
    }

    private void EnsureDb()
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Submissions (
  Id TEXT PRIMARY KEY,
  StudentName TEXT,
  AssignmentId TEXT,
  FileName TEXT,
  FilePath TEXT,
  UploadedAt TEXT
);";
        cmd.ExecuteNonQuery();
    }

    public async Task AddAsync(Submission submission, CancellationToken ct = default)
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        await conn.OpenAsync(ct);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Submissions(Id, StudentName, AssignmentId, FileName, FilePath, UploadedAt)
VALUES($id,$student,$assignment,$fname,$fpath,$uploadedAt)";
        cmd.Parameters.AddWithValue("$id", submission.Id);
        cmd.Parameters.AddWithValue("$student", submission.StudentName);
        cmd.Parameters.AddWithValue("$assignment", submission.AssignmentId);
        cmd.Parameters.AddWithValue("$fname", submission.FileName);
        cmd.Parameters.AddWithValue("$fpath", submission.FilePath);
        cmd.Parameters.AddWithValue("$uploadedAt", submission.UploadedAt.ToString("o"));
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<Submission?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        await conn.OpenAsync(ct);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, StudentName, AssignmentId, FileName, FilePath, UploadedAt FROM Submissions WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var rdr = await cmd.ExecuteReaderAsync(ct);
        if (!await rdr.ReadAsync(ct)) return null;
        return new Submission(
            rdr.GetString(0),
            rdr.GetString(1),
            rdr.GetString(2),
            rdr.GetString(3),
            rdr.GetString(4),
            DateTime.Parse(rdr.GetString(5))
        );
    }

    public async Task<IReadOnlyList<Submission>> GetByAssignmentAsync(string assignmentId, CancellationToken ct = default)
    {
        var list = new List<Submission>();
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        await conn.OpenAsync(ct);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, StudentName, AssignmentId, FileName, FilePath, UploadedAt FROM Submissions WHERE AssignmentId=$assignmentId ORDER BY UploadedAt ASC";
        cmd.Parameters.AddWithValue("$assignmentId", assignmentId);
        using var rdr = await cmd.ExecuteReaderAsync(ct);
        while (await rdr.ReadAsync(ct))
        {
            list.Add(new Submission(
                rdr.GetString(0),
                rdr.GetString(1),
                rdr.GetString(2),
                rdr.GetString(3),
                rdr.GetString(4),
                DateTime.Parse(rdr.GetString(5))
            ));
        }
        return list;
    }
}
