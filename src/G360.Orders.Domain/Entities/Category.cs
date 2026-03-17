using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Domain.Entities;

/// <summary>
/// Represents a category entity (e.g. for grouping pizza types).
/// </summary>
public class Category : IEntity, IAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public bool IsDeleted { get; set; }
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }

    public virtual ICollection<PizzaType> PizzaTypes { get; set; } = new List<PizzaType>();
}
