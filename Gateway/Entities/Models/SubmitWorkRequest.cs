namespace Gateway.Entities.Models;

public class SubmitWorkRequest
{
    public IFormFile File { get; set; } = default!;
    public string StudentName { get; set; } = default!;
    public string AssignmentId { get; set; } = default!;
}