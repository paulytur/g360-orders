using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Domain.Entities;

/// <summary>
/// Represents a pizza entity.
/// </summary>
public class Pizza : IEntity, IAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Code { get; set; }

    [StringLength(50)]
    public string? Size { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public long? PizzaTypeId { get; set; }
    public virtual PizzaType? PizzaType { get; set; }

    public bool IsDeleted { get; set; }
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }
}
