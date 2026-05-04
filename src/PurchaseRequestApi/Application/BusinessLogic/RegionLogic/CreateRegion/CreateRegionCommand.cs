using Application.BusinessLogic.RegionLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RegionLogic.CreateRegion
{
    public record CreateRegionCommand(CrudRegionDto dto) : IRequest<Result<CrudRegionDto>>;
}