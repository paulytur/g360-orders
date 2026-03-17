namespace G360.Orders.Domain.DTO;

/// <summary>
/// DTO for updating an order detail line.
/// </summary>
public record OrderDetailUpdate(long PizzaId, int Quantity);

