using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer.DTOs
{
    public class BinaryQuantityRequestDto
    {
        public QuantityDTO FirstQuantity { get; set; } = new QuantityDTO();
        public QuantityDTO SecondQuantity { get; set; } = new QuantityDTO();
        public string? TargetUnit { get; set; }
    }
}
