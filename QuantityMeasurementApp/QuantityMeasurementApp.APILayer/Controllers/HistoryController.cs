using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.RepositoryLayer.Records;

namespace QuantityMeasurementApp.APILayer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController:ControllerBase
    {
        private readonly IQuantityApplicationService _historyService;
        private readonly ILogger<HistoryController> _logger;

        public HistoryController(IQuantityApplicationService historyService,ILogger<HistoryController> logger)
        {
            _historyService=historyService;
            _logger=logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddRecord([FromBody] QuantityHistoryRecord record)
        {
            _logger.LogInformation("Add record {Category} {Operation}",record.Category,record.OperationType);
            await _historyService.AddRecordAsync(record);
            return Ok("Record added");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetch all records");
            var data=await _historyService.GetAllRecordsAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetch record {Id}",id);
            var data=await _historyService.GetRecordByIdAsync(id);
            if(data==null) return NotFound($"No record for id {id}");
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete record {Id}",id);
            var deleted=await _historyService.DeleteRecordAsync(id);
            if(!deleted) return NotFound();
            return NoContent();
        }
    }
}