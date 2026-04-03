namespace Shared.Contracts;

// ──────────────────────────────────────────────
// Request DTOs (used by Quantity Service)
// ──────────────────────────────────────────────

public record QuantityDto(double Value, string Unit);

public record ConversionRequestDto(QuantityDto SourceQuantity, string TargetUnit);

public record BinaryQuantityRequestDto(
    QuantityDto FirstQuantity,
    QuantityDto SecondQuantity,
    string? TargetUnit = null);

// ──────────────────────────────────────────────
// Response DTOs (used by Quantity Service)
// ──────────────────────────────────────────────

public record QuantityResultDto(double Value, string Unit, string Message);

public record DivisionResultDto(double Value, string Message);

public record EqualityResultDto(bool AreEqual, string Message);

// ──────────────────────────────────────────────
// Auth DTOs (used by Auth Service)
// ──────────────────────────────────────────────

public record RegisterRequestDto(string Email, string Password);

public record LoginRequestDto(string Email, string Password);

public record AuthResponseDto(string Token, string Email, DateTime ExpiresAt);

// ──────────────────────────────────────────────
// History DTOs (used by History Service)
// ──────────────────────────────────────────────

public record QuantityHistoryRecordDto(
    int Id,
    string Category,
    string OperationType,
    double FirstValue,
    string FirstUnit,
    double? SecondValue,
    string? SecondUnit,
    string? TargetUnit,
    double ResultValue,
    string ResultUnit,
    DateTime CreatedAt);

public record CreateHistoryRecordDto(
    string Category,
    string OperationType,
    double FirstValue,
    string FirstUnit,
    double? SecondValue,
    string? SecondUnit,
    string? TargetUnit,
    double ResultValue,
    string ResultUnit);
