using System;
using System.Collections.Generic;
using System.Text;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Entity;
namespace QuantityMeasurementApp.BusinessLayer.Mappers
{
    public class QuantityDtoMapper
    {
        public static Quantity<T> ToEntity<T>(QuantityDTO dto) where T : struct, Enum
        {
            if(dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            if(!Enum.TryParse(dto.Unit,true,out T unit))
            {
                throw new ArgumentException($"Invalid unit '{dto.Unit}' for {typeof(T).Name}.");
            }
            return new Quantity<T>(dto.Value, unit);
        }
        public static T ParseTargetUnit<T>(string unitText) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(unitText))
                throw new ArgumentException("Target unit cannot be empty.");

            if (!Enum.TryParse(unitText, true, out T unit))
                throw new ArgumentException($"Invalid unit '{unitText}' for {typeof(T).Name}.");

            return unit;
        }
        public static QuantityResultDto ToResultDto<T>(Quantity<T> quantity, string message) where T : struct, Enum
        {
            return new QuantityResultDto
            {
                Value = quantity.Value,
                Unit = quantity.Unit.ToString(),
                Message = message
            };
        }
    }
}
