using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer.DTOs
{
    public class QuantityResultDto
    {
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
