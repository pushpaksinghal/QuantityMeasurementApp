using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryService.Models;

[Table("QuantityHistory")]
public class QuantityHistoryEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string OperationType { get; set; } = string.Empty;

    public double FirstValue { get; set; }

    [Required, MaxLength(50)]
    public string FirstUnit { get; set; } = string.Empty;

    public double? SecondValue { get; set; }

    [MaxLength(50)]
    public string? SecondUnit { get; set; }

    [MaxLength(50)]
    public string? TargetUnit { get; set; }

    public double ResultValue { get; set; }

    [Required, MaxLength(50)]
    public string ResultUnit { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
