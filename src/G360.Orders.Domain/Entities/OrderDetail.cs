using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Domain.Entities;

/// <summary>
/// Represents an order detail line (order item with pizza and quantity).
/// </summary>
public class OrderDetail : IEntity, IAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long OrderId { get; set; }
    public virtual Order? Order { get; set; }

    public long PizzaId { get; set; }
    public virtual Pizza? Pizza { get; set; }

    public int Quantity { get; set; }

    public bool IsDeleted { get; set; }
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }
}
