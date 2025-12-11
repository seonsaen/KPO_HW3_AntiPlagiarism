using FileStoring.Entities.Models;
using FileStoring.Repositories;

namespace FileStoring.Services;

public interface IFileService
{
    Task<Submission> SaveFileAsync(IFormFile file, string studentName, string assignmentId, CancellationToken ct = default);
    Task<Stream?> OpenReadAsync(string filePath, CancellationToken ct = default);
}

public class FileService : IFileService
{
    private readonly string _filesDir;
    private readonly ISubmissionRepository _repo;
    public FileService(IConfiguration config, ISubmissionRepository repo)
    {
        _repo = repo;
        var dataDir = config.GetValue<string>("Storage:DataDir") ?? "/app/data";
        _filesDir = Path.Combine(dataDir, "files");
        Directory.CreateDirectory(_filesDir);
    }

    public async Task<Submission> SaveFileAsync(IFormFile file, string studentName, string assignmentId, CancellationToken ct = default)
    {
        var id = Guid.NewGuid().ToString();
        var filename = Path.GetFileName(file.FileName);
        var storedName = $"{id}_{filename}";
        var storedPath = Path.Combine(_filesDir, storedName);

        await using (var fs = File.Create(storedPath))
        {
            await file.CopyToAsync(fs, ct);
        }

        var submission = new Submission(id, studentName, assignmentId, filename, storedPath, DateTime.UtcNow);
        await _repo.AddAsync(submission, ct);
        return submission;
    }

    public Task<Stream?> OpenReadAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath)) return Task.FromResult<Stream?>(null);
        var stream = File.OpenRead(filePath);
        return Task.FromResult<Stream?>(stream);
    }
}
