using QuantityMeasurementApp.RepositoryLayer.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.RepositoryLayer.Interfaces
{
    public interface IQuantityHistoryRepository
    {
        void AddRecord(QuantityHistoryRecord record);
        List<QuantityHistoryRecord> GetAllRecords();
        QuantityHistoryRecord? GetRecordById(int id);
        bool DeleteRecord(int id);
    }
}
