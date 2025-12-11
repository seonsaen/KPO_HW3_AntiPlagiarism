namespace FileStoring.Entities.Models;

public record Submission(
    string Id,
    string StudentName,
    string AssignmentId,
    string FileName,
    string FilePath,
    DateTime UploadedAt
);