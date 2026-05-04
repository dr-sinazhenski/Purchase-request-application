using Application.BusinessLogic.RegionLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RegionLogic.GetAllRegions
{
    public record GetAllRegionsCommand() : IRequest<Result<List<CrudRegionDto>>>;
}