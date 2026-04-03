using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HistoryService.Repositories;
using Shared.Contracts;

namespace HistoryService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private readonly HistoryApplicationService _service;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(HistoryApplicationService service, ILogger<HistoryController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    // GET api/history
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll history records");
        var records = await _service.GetAllAsync();
        return Ok(records);
    }

    // GET api/history/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GetById {Id}", id);
        var record = await _service.GetByIdAsync(id);
        return record is null ? NotFound($"Record {id} not found.") : Ok(record);
    }

    // POST api/history  — called internally by Quantity Service (still auth-protected)
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateHistoryRecordDto request)
    {
        _logger.LogInformation("AddRecord category={Category} op={Op}", request.Category, request.OperationType);
        var created = await _service.AddAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE api/history/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Delete record {Id}", id);
        bool deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound($"Record {id} not found.");
    }
}
