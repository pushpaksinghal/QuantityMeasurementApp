using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer.DTOs
{
    public class EqualityResultDto
    {
        public bool AreEqual { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
