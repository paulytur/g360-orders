namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Interface for entities that support audit fields (soft delete and audit trail).
/// </summary>
public interface IAuditableEntity
{
    bool IsDeleted { get; set; }
    string? CreatedBy { get; set; }
    DateTime CreatedDatetime { get; set; }
    string? UpdatedBy { get; set; }
    DateTime UpdatedDatetime { get; set; }
}
