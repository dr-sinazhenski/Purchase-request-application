using MediatR;
using Shared;

namespace Application.BusinessLogic.PriceLogic.DeletePrice
{
    public record DeletePriceCommand(Guid ProductId, Guid RegionId) : IRequest<Result>;
}