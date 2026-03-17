using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Domain.Entities;

/// <summary>
/// Represents a pizza type entity.
/// </summary>
public class PizzaType : IEntity, IAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Code { get; set; }

    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    public long? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
    public virtual ICollection<PizzaDetail> PizzaDetails { get; set; } = new List<PizzaDetail>();

    public bool IsDeleted { get; set; }
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }
}
