using QuantityMeasurementApp.BusinessLayer.Mappers;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Utility;
using QuantityMeasurementApp.RepositoryLayer.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public interface IQuantityApplicationService
    {
        EqualityResultDto CheckEquality<T>(BinaryQuantityRequestDto request, QuantityEqualityComparer<T> equalityComparer) where T : struct, Enum;
        QuantityResultDto ConvertUnit<T>(ConversionRequestDto request) where T : struct, Enum;
        QuantityResultDto AddUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        QuantityResultDto AddUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        QuantityResultDto SubtractUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        QuantityResultDto SubtractUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        DivisionResultDto DivideUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;

    }
    public class QuantityApplicationService: IQuantityApplicationService
    {
        private readonly IQuantityConversionService _conversionService;
        private readonly IQuantityArithmeticService _arithmeticService;
        private readonly IQuantityHistoryRepository _historyRepository;

        public QuantityApplicationService(
            IQuantityConversionService conversionService,
            IQuantityArithmeticService arithmeticService,
            IQuantityHistoryRepository historyRepository)
        {
            _conversionService = conversionService;
            _arithmeticService = arithmeticService;
            _historyRepository = historyRepository;
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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Conversion",
                FirstValue = source.Value,
                FirstUnit = source.Unit.ToString(),
                SecondValue = null,
                SecondUnit = null,
                TargetUnit =targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString()
            });

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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Addition",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString()
            });

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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Addition",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString()
            });

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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Subtraction",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString()
            });

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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Subtraction",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString()
            });

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

            _historyRepository.AddRecord(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", ""),
                OperationType = "Divide",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result,
                ResultUnit = "Dimensionless"
            });
            return new DivisionResultDto
            {
                Value = result,
                Message = $"{q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {result:F4} (dimensionless)"
            };
        }
    }
}
