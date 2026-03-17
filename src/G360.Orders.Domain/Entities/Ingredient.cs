using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Domain.Entities;

/// <summary>
/// Represents an ingredient entity.
/// </summary>
public class Ingredient : IEntity, IAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(500)]
    public required string Description { get; set; }

    public bool IsDeleted { get; set; }
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }
}
