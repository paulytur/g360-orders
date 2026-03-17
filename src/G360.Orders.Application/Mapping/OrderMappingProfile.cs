using AutoMapper;
using G360.Orders.Application.Models;
using G360.Orders.Domain.Entities;

namespace G360.Orders.Application.Mapping;

/// <summary>
/// AutoMapper profile for Order, OrderDetail, PizzaType (response DTOs).
/// </summary>
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(d => d.OrderDetails, o => o.MapFrom(s => s.OrderDetails ?? new List<OrderDetail>()));
        CreateMap<OrderDetail, OrderDetailResponse>();

        CreateMap<Category, CategoryResponse>();
        CreateMap<PizzaType, PizzaTypeResponse>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category))
            .ForMember(d => d.Ingredients, o => o.MapFrom(s =>
                (s.PizzaDetails ?? new List<PizzaDetail>())
                .Where(pd => !pd.IsDeleted && pd.Ingredient !=  null)
                .Select(pd => new PizzaTypeIngredientResponse
                {
                    IngredientId = pd.IngredientId,
                    Description = pd.Ingredient!.Description
                })
                .ToList()));
    }
}
