using Application.BusinessLogic.RegionLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RegionLogic.GetRegion
{
    public record GetRegionCommand(Guid Id) : IRequest<Result<CrudRegionDto>>;
}