using Gateway.Entities.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("works")]
public class WorksController : ControllerBase
{
    private readonly IGatewayService _service;
    public WorksController(IGatewayService service) => _service = service;

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromForm] SubmitWorkRequest request, CancellationToken ct)
    {
        var result = await _service.SubmitAndAnalyzeAsync(
            request.File,
            request.StudentName,
            request.AssignmentId,
            ct
        );

        return Ok(result);
    }

    [HttpGet("{assignmentId}/reports")]
    public async Task<IActionResult> Reports(string assignmentId, CancellationToken ct)
    {
        var result = await _service.GetReportsAsync(assignmentId, ct);
        return Ok(result);
    }
}
