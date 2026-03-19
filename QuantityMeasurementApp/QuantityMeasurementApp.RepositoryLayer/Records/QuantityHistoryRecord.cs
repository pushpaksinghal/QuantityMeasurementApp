using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.RepositoryLayer.Records
{
    public class QuantityHistoryRecord
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty;
        public double FirstValue { get; set; }
        public string FirstUnit { get; set; } = string.Empty;
        public double? SecondValue { get; set; }
        public string? SecondUnit { get; set; }
        public string? TargetUnit { get; set; }
        public double ResultValue { get; set; }
        public string ResultUnit { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return "ID: " + Id + " | CATEGORY: " + Category + " | OPERATION TYPE: " + OperationType + " | FIRST VALUE: " + FirstValue + " | FIRST UNIT: " + FirstUnit + " | SECOND VALUE: " + SecondValue + " | SECOND UNIT: " + SecondUnit + " | TARGET UNIT: " + TargetUnit + " | RESULT: " + ResultValue + " " + ResultUnit + " | CREATED AT: " + CreatedAt;
        }
    }
}
