using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityService.Services;
using QuantityService.Models;
using Shared.Contracts;

namespace QuantityService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuantityController : ControllerBase
{
    private readonly QuantityOrchestrator _orchestrator;
    private readonly ILogger<QuantityController> _logger;

    public QuantityController(QuantityOrchestrator orchestrator, ILogger<QuantityController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    // POST api/quantity/{type}/convert
    [HttpPost("{type}/convert")]
    public ActionResult<QuantityResultDto> Convert(string type, [FromBody] ConversionRequestDto request)
    {
        _logger.LogInformation("Convert called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.ConvertUnit<LengthUnit>(request),
            () => _orchestrator.ConvertUnit<WeightUnit>(request),
            () => _orchestrator.ConvertUnit<VolumeUnit>(request),
            () => _orchestrator.ConvertUnit<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/same-add
    [HttpPost("{type}/same-add")]
    public ActionResult<QuantityResultDto> SameAdd(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("SameAdd called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.Add<LengthUnit>(request),
            () => _orchestrator.Add<WeightUnit>(request),
            () => _orchestrator.Add<VolumeUnit>(request),
            () => _orchestrator.Add<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/target-add
    [HttpPost("{type}/target-add")]
    public ActionResult<QuantityResultDto> TargetAdd(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("TargetAdd called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.AddToTarget<LengthUnit>(request),
            () => _orchestrator.AddToTarget<WeightUnit>(request),
            () => _orchestrator.AddToTarget<VolumeUnit>(request),
            () => _orchestrator.AddToTarget<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/same-subtract
    [HttpPost("{type}/same-subtract")]
    public ActionResult<QuantityResultDto> SameSubtract(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("SameSubtract called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.Subtract<LengthUnit>(request),
            () => _orchestrator.Subtract<WeightUnit>(request),
            () => _orchestrator.Subtract<VolumeUnit>(request),
            () => _orchestrator.Subtract<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/target-subtract
    [HttpPost("{type}/target-subtract")]
    public ActionResult<QuantityResultDto> TargetSubtract(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("TargetSubtract called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.SubtractToTarget<LengthUnit>(request),
            () => _orchestrator.SubtractToTarget<WeightUnit>(request),
            () => _orchestrator.SubtractToTarget<VolumeUnit>(request),
            () => _orchestrator.SubtractToTarget<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/divide
    [HttpPost("{type}/divide")]
    public ActionResult<DivisionResultDto> Divide(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("Divide called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.Divide<LengthUnit>(request),
            () => _orchestrator.Divide<WeightUnit>(request),
            () => _orchestrator.Divide<VolumeUnit>(request),
            () => _orchestrator.Divide<TemperatureUnit>(request));
        return Ok(result);
    }

    // POST api/quantity/{type}/equals
    [HttpPost("{type}/equals")]
    public ActionResult<EqualityResultDto> EqualsCheck(string type, [FromBody] BinaryQuantityRequestDto request)
    {
        _logger.LogInformation("EqualsCheck called for type {Type}", type);
        var result = Dispatch(type,
            () => _orchestrator.CheckEquality<LengthUnit>(request),
            () => _orchestrator.CheckEquality<WeightUnit>(request),
            () => _orchestrator.CheckEquality<VolumeUnit>(request),
            () => _orchestrator.CheckEquality<TemperatureUnit>(request));
        return Ok(result);
    }

    // ── helper: route "length/weight/volume/temperature" to correct generic ──
    private static T Dispatch<T>(
        string type,
        Func<T> length,
        Func<T> weight,
        Func<T> volume,
        Func<T> temperature) =>
        type.ToLowerInvariant() switch
        {
            "length"      => length(),
            "weight"      => weight(),
            "volume"      => volume(),
            "temperature" => temperature(),
            _             => throw new ArgumentException($"Unknown quantity type: '{type}'.")
        };
}
