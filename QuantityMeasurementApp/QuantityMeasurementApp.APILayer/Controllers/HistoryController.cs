using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using System.Runtime.CompilerServices;

namespace QuantityMeasurementApp.APILayer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController:ControllerBase
    {
        private readonly IQuantityApplicationService _historyService;
        private readonly ILogger<HistoryController> _logger;

        public HistoryController(IQuantityApplicationService historyService, ILogger<HistoryController> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddRecord([FromBody] QuantityHistoryRecord record)
        {
            _logger.LogInformation("AddRecord called for Category {Category} and Operation {OperationType}",
                record.Category, record.OperationType);
            await _historyService.AddRecordAsync(record);
            _logger.LogInformation("Record added successfully");
            return Ok("Record added successfully");
        }

        [HttpGet]
        public async Task <IActionResult> GetAll()
        {
            
                _logger.LogInformation("GetAll history records called");
                var data = await _historyService.GetAllRecordsAsync();
                _logger.LogInformation("GetAll returned {Count} records", data.Count);
                return Ok(data);
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GetById called with Id {Id}", id);
            var data =await  _historyService.GetRecordByIdAsync(id);

            if(data == null)
            {
                _logger.LogWarning("Record with Id {Id} not found", id);
                return NotFound($"Record with id {id} not found");
            }
            _logger.LogInformation("Record with Id {Id} returned successfully", id);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete called for Id {Id}", id);
            bool deleted = await _historyService.DeleteRecordAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Delete failed. Record with Id {Id} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Record with Id {Id} deleted successfully", id);
            return NoContent();
        }
    }
}
