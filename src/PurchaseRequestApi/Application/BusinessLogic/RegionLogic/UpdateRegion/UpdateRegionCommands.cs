using Application.BusinessLogic.RegionLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RegionLogic.UpdateRegion
{
    public record UpdateRegionCommand(CrudRegionDto dto) : IRequest<Result<CrudRegionDto>>;
}