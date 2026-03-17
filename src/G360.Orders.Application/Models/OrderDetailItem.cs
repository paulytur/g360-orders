namespace G360.Orders.Application.Models;

/// <summary>
/// Represents an order line item (pizza + quantity).
/// </summary>
public class OrderDetailItem
{
    public long PizzaId { get; set; }
    public int Quantity { get; set; }
}
