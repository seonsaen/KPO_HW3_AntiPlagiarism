using FileStoring.Entities.Models;

namespace FileStoring.Repositories;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission, CancellationToken ct = default);
    Task<Submission?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Submission>> GetByAssignmentAsync(string assignmentId, CancellationToken ct = default);
}
