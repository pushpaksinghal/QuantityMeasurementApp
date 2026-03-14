using QuantityMeasurementApp.BusinessLayer.Mappers;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class QuantityApplicationService
    {
        private readonly IQuantityConversionService _conversionService;
        private readonly IQuantityArithmeticService _arithmeticService;

        public QuantityApplicationService(
            IQuantityConversionService conversionService,
            IQuantityArithmeticService arithmeticService)
        {
            _conversionService = conversionService;
            _arithmeticService = arithmeticService;
        }

        public EqualityResultDto CheckEquality<T>(
            BinaryQuantityRequestDto request,
            QuantityEqualityComparer<T> equalityComparer) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            bool areEqual = equalityComparer.Equals(q1, q2);

            return new EqualityResultDto
            {
                AreEqual = areEqual,
                Message = $"{q1.Value} {q1.Unit} {(areEqual ? "==" : "!=")} {q2.Value} {q2.Unit}"
            };
        }

        public QuantityResultDto ConvertUnit<T>(ConversionRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> source = QuantityDtoMapper.ToEntity<T>(request.SourceQuantity);
            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);

            Quantity<T> result = _conversionService.ConvertTo(source, targetUnit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{source.Value} {source.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto AddUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            Quantity<T> result = _arithmeticService.AddUnit(q1, q2);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto AddUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            if (string.IsNullOrWhiteSpace(request.TargetUnit))
                throw new ArgumentException("Target unit is required.");

            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);

            Quantity<T> result = _arithmeticService.AddToSpecificUnit(q1, q2, targetUnit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto SubtractUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            Quantity<T> result = _arithmeticService.SubtractUnit(q1, q2, q1.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto SubtractUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            if (string.IsNullOrWhiteSpace(request.TargetUnit))
                throw new ArgumentException("Target unit is required.");

            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);

            Quantity<T> result = _arithmeticService.SubtractUnit(q1, q2, targetUnit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public DivisionResultDto DivideUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            double result = _arithmeticService.DivideUnit(q1, q2);

            return new DivisionResultDto
            {
                Value = result,
                Message = $"{q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {result:F4} (dimensionless)"
            };
        }
    }
}
