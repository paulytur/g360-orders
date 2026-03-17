using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to soft-delete a pizza.
/// </summary>
public class DeletePizzaCommand : IRequest<Response>
{
    public long Id { get; set; }
}
