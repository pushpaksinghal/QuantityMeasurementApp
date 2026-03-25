using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer.DTOs
{
    public class ConversionRequestDto
    {
        public QuantityDTO SourceQuantity { get; set; } = new QuantityDTO();
        public string TargetUnit { get; set; } = string.Empty;
    }
}
