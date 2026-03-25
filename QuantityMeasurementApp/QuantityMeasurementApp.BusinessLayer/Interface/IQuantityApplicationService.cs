using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.RepositoryLayer.Records;

namespace QuantityMeasurementApp.BusinessLayer.Interface
{
    public interface IQuantityApplicationService
    {
        public EqualityResultDto CheckEquality<T>(BinaryQuantityRequestDto request, QuantityEqualityComparer<T> equalityComparer) where T : struct, Enum;
        public QuantityResultDto ConvertUnit<T>(ConversionRequestDto request) where T : struct, Enum;
        public QuantityResultDto AddUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        public QuantityResultDto AddUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        public QuantityResultDto SubtractUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        public QuantityResultDto SubtractUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        public DivisionResultDto DivideUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum;
        
        public Task<List<QuantityHistoryRecord>> GetAllRecordsAsync();
        public Task<QuantityHistoryRecord?> GetRecordByIdAsync(int id);
        public Task AddRecordAsync(QuantityHistoryRecord record);
        public Task<bool> DeleteRecordAsync(int id);

    }
}
