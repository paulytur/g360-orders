using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to soft-delete an ingredient.
/// </summary>
public class DeleteIngredientCommand : IRequest<Response>
{
    public long Id { get; set; }
}
