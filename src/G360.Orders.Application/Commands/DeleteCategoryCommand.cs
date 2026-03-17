using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Application.Commands;

public class DeleteCategoryCommand : IRequest<Response>
{
    public long Id { get; set; }
}
