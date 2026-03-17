using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve an ingredient by its identifier.
/// </summary>
public class GetIngredientByIdQuery : IRequest<Response<Ingredient>>
{
    public long Id { get; set; }
}
