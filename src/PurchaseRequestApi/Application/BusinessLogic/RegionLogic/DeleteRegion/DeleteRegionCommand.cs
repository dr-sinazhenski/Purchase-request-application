using MediatR;
using Shared;

namespace Application.BusinessLogic.RegionLogic.DeleteRegion
{
    public record DeleteRegionCommand(Guid Id) : IRequest<Result<bool>>;
}