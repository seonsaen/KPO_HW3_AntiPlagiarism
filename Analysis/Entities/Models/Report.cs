namespace FileAnalysisService.Entities.Models;

public record Report(
    string Id,
    string SubmissionId,
    string StudentName,
    string AssignmentId,
    bool IsPlagiarism,
    string? SimilarSubmissionId,
    DateTime CreatedAt,
    string? ReportPath,
    string? WordCloudUrl
);