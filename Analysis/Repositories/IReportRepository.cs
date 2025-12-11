using FileAnalysisService.Entities.Models;

namespace FileAnalysisService.Repositories;

public interface IReportRepository
{
    Task AddAsync(Report report, CancellationToken ct = default);
    Task<IReadOnlyList<Report>> GetByAssignmentAsync(string assignmentId, CancellationToken ct = default);
}
