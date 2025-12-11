namespace FileStoring.Entities.Models;

public class UploadRequest
{
    public IFormFile File { get; set; } = default!;
    public string StudentName { get; set; } = default!;
    public string AssignmentId { get; set; } = default!;
}