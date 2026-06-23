using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.CreateRequest
{
    public record CreateRequestCommand(CreateRequestDto dto, Guid RequesterId) : IRequest<Result<GetRequestDetailsResDto>>;
}
