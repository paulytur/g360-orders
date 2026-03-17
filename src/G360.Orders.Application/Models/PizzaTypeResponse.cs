namespace G360.Orders.Application.Models;

public class PizzaTypeResponse
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CategoryResponse? Category { get; set; }
    public List<PizzaTypeIngredientResponse> Ingredients { get; set; } = [];
}
