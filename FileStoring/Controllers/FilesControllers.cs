using FileStoring.Entities.Models;
using FileStoring.Repositories;
using FileStoring.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStoring.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ISubmissionRepository _repo;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileService fileService, ISubmissionRepository repo, ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _repo = repo;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadRequest request, CancellationToken ct)
    {
        var submission = await _fileService.SaveFileAsync(
            request.File,
            request.StudentName,
            request.AssignmentId,
            ct
        );

        return Ok(new
        {
            submissionId = submission.Id,
            studentName = submission.StudentName,
            assignmentId = submission.AssignmentId,
            uploadedAt = submission.UploadedAt.ToString("o")
        });
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(string id, CancellationToken ct)
    {
        var meta = await _repo.GetByIdAsync(id, ct);
        if (meta == null) return NotFound();
        var stream = await _fileService.OpenReadAsync(meta.FilePath, ct);
        if (stream == null) return NotFound();
        return File(stream, "application/octet-stream", meta.FileName, enableRangeProcessing: true);
    }

    [HttpGet("{id}/metadata")]
    public async Task<IActionResult> Metadata(string id, CancellationToken ct)
    {
        var meta = await _repo.GetByIdAsync(id, ct);
        if (meta == null) return NotFound();
        return Ok(new {
            id = meta.Id,
            studentName = meta.StudentName,
            assignmentId = meta.AssignmentId,
            fileName = meta.FileName,
            filePath = meta.FilePath,
            uploadedAt = meta.UploadedAt.ToString("o")
        });
    }

    [HttpGet("works/{assignmentId}/submissions")]
    public async Task<IActionResult> Submissions(string assignmentId, CancellationToken ct)
    {
        var list = await _repo.GetByAssignmentAsync(assignmentId, ct);
        var dto = list.Select(s => new {
            id = s.Id,
            studentName = s.StudentName,
            assignmentId = s.AssignmentId,
            fileName = s.FileName,
            filePath = s.FilePath,
            uploadedAt = s.UploadedAt.ToString("o")
        });
        return Ok(dto);
    }
}