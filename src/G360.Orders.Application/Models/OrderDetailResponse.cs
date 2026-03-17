namespace G360.Orders.Application.Models;

public class OrderDetailResponse
{
    public long Id { get; set; }
    public long PizzaId { get; set; }
    public int Quantity { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedDatetime { get; set; }
}
