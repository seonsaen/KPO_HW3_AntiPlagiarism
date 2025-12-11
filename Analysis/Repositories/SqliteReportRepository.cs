using FileAnalysisService.Entities.Models;
using Microsoft.Data.Sqlite;

namespace FileAnalysisService.Repositories;

public class SqliteReportRepository : IReportRepository
{
    private readonly string _dbPath;
    public SqliteReportRepository(IConfiguration config)
    {
        var dataDir = config.GetValue<string>("Storage:DataDir") ?? "/app/data";
        Directory.CreateDirectory(dataDir);
        _dbPath = Path.Combine(dataDir, "analysis.db");
        EnsureDb();
    }

    private void EnsureDb()
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Reports (
  Id TEXT PRIMARY KEY,
  SubmissionId TEXT,
  StudentName TEXT,
  AssignmentId TEXT,
  IsPlagiarism INTEGER,
  SimilarSubmissionId TEXT,
  CreatedAt TEXT,
  ReportPath TEXT,
  WordCloudUrl TEXT
);";
        cmd.ExecuteNonQuery();
    }

    public async Task AddAsync(Report report, CancellationToken ct = default)
    {
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        await conn.OpenAsync(ct);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Reports(Id, SubmissionId, StudentName, AssignmentId, IsPlagiarism, SimilarSubmissionId, CreatedAt, ReportPath, WordCloudUrl)
VALUES($id,$sub,$student,$assign,$isplag,$sim,$created,$path,$wc)";
        cmd.Parameters.AddWithValue("$id", report.Id);
        cmd.Parameters.AddWithValue("$sub", report.SubmissionId);
        cmd.Parameters.AddWithValue("$student", report.StudentName);
        cmd.Parameters.AddWithValue("$assign", report.AssignmentId);
        cmd.Parameters.AddWithValue("$isplag", report.IsPlagiarism ? 1 : 0);
        cmd.Parameters.AddWithValue("$sim", (object?)report.SimilarSubmissionId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$created", report.CreatedAt.ToString("o"));
        cmd.Parameters.AddWithValue("$path", (object?)report.ReportPath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$wc", (object?)report.WordCloudUrl ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Report>> GetByAssignmentAsync(string assignmentId, CancellationToken ct = default)
    {
        var list = new List<Report>();
        using var conn = new SqliteConnection($"Data Source={_dbPath}");
        await conn.OpenAsync(ct);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, SubmissionId, StudentName, AssignmentId, IsPlagiarism, SimilarSubmissionId, CreatedAt, ReportPath, WordCloudUrl FROM Reports WHERE AssignmentId=$assignmentId ORDER BY CreatedAt ASC";
        cmd.Parameters.AddWithValue("$assignmentId", assignmentId);
        using var rdr = await cmd.ExecuteReaderAsync(ct);
        while (await rdr.ReadAsync(ct))
        {
            list.Add(new Report(
                rdr.GetString(0),
                rdr.GetString(1),
                rdr.GetString(2),
                rdr.GetString(3),
                rdr.GetInt32(4) == 1,
                rdr.IsDBNull(5) ? null : rdr.GetString(5),
                DateTime.Parse(rdr.GetString(6)),
                rdr.IsDBNull(7) ? null : rdr.GetString(7),
                rdr.IsDBNull(8) ? null : rdr.GetString(8)
            ));
        }
        return list;
    }
}
