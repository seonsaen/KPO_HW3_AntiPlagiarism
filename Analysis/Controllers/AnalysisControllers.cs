using FileAnalysisService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers; 

[ApiController]
[Route("analysis")]
public class AnalysisController : ControllerBase
{
    private readonly IAnalysisService _service;
    public AnalysisController(IAnalysisService service) => _service = service;

    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] TriggerDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.SubmissionId)) return BadRequest("submissionId required");
        var report = await _service.AnalyzeAsync(dto.SubmissionId, ct);
        return Ok(report);
    }

    [HttpGet("reports/by-work/{assignmentId}")]
    public async Task<IActionResult> ReportsByWork(string assignmentId, CancellationToken ct)
    {
        var list = await _service.GetReportsByAssignmentAsync(assignmentId, ct);
        return Ok(list);
    }

    public record TriggerDto(string SubmissionId);
}