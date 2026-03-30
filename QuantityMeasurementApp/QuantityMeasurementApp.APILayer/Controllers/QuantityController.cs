using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Enums;

namespace QuantityMeasurementApp.APILayer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuantityController:ControllerBase
    {
        private readonly IQuantityApplicationService _quantityService;
        private readonly UnitAdapterFactory _adapterFactory;
        private readonly QuantityValidationService _validator;
        private readonly ILogger<QuantityController> _logger;

        public QuantityController(IQuantityApplicationService quantityService,UnitAdapterFactory adapterFactory,QuantityValidationService validator,ILogger<QuantityController> logger)
        {
            _quantityService=quantityService;
            _adapterFactory=adapterFactory;
            _validator=validator;
            _logger=logger;
        }

        [HttpPost("{type}/convert")]
        public ActionResult<QuantityResultDto> Convert(string type,[FromBody] ConversionRequestDto request)
        {
            _logger.LogInformation("Convert {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.ConvertUnit<LengthUnit>(request),
                "weight"=>_quantityService.ConvertUnit<WeightUnit>(request),
                "volume"=>_quantityService.ConvertUnit<VolumeUnit>(request),
                "temperature"=>_quantityService.ConvertUnit<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/same-add")]
        public ActionResult<QuantityResultDto> SameAdd(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Same add {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.AddUnits<LengthUnit>(request),
                "weight"=>_quantityService.AddUnits<WeightUnit>(request),
                "volume"=>_quantityService.AddUnits<VolumeUnit>(request),
                "temperature"=>_quantityService.AddUnits<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/target-add")]
        public ActionResult<QuantityResultDto> TargetAdd(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Target add {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.AddUnitsToTarget<LengthUnit>(request),
                "weight"=>_quantityService.AddUnitsToTarget<WeightUnit>(request),
                "volume"=>_quantityService.AddUnitsToTarget<VolumeUnit>(request),
                "temperature"=>_quantityService.AddUnitsToTarget<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/same-subtract")]
        public ActionResult<QuantityResultDto> SameSubtract(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Same subtract {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.SubtractUnits<LengthUnit>(request),
                "weight"=>_quantityService.SubtractUnits<WeightUnit>(request),
                "volume"=>_quantityService.SubtractUnits<VolumeUnit>(request),
                "temperature"=>_quantityService.SubtractUnits<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/target-subtract")]
        public ActionResult<QuantityResultDto> TargetSubtract(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Target subtract {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.SubtractUnitsToTarget<LengthUnit>(request),
                "weight"=>_quantityService.SubtractUnitsToTarget<WeightUnit>(request),
                "volume"=>_quantityService.SubtractUnitsToTarget<VolumeUnit>(request),
                "temperature"=>_quantityService.SubtractUnitsToTarget<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/divide")]
        public ActionResult<DivisionResultDto> Divide(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Divide {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.DivideUnits<LengthUnit>(request),
                "weight"=>_quantityService.DivideUnits<WeightUnit>(request),
                "volume"=>_quantityService.DivideUnits<VolumeUnit>(request),
                "temperature"=>_quantityService.DivideUnits<TemperatureUnit>(request),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }

        [HttpPost("{type}/equals")]
        public ActionResult<bool> EqualsCheck(string type,[FromBody] BinaryQuantityRequestDto request)
        {
            _logger.LogInformation("Equals check {Type}",type);
            var result=type.ToLower() switch
            {
                "length"=>_quantityService.CheckEquality(request,new QuantityEqualityComparer<LengthUnit>(_adapterFactory,_validator)),
                "weight"=>_quantityService.CheckEquality(request,new QuantityEqualityComparer<WeightUnit>(_adapterFactory,_validator)),
                "volume"=>_quantityService.CheckEquality(request,new QuantityEqualityComparer<VolumeUnit>(_adapterFactory,_validator)),
                "temperature"=>_quantityService.CheckEquality(request,new QuantityEqualityComparer<TemperatureUnit>(_adapterFactory,_validator)),
                _=>throw new ArgumentException("Invalid type")
            };
            return Ok(result);
        }
    }
}