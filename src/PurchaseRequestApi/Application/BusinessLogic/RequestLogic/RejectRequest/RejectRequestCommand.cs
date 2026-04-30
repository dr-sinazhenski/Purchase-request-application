using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.RejectRequest
{
    public record RejectRequestCommand(RejectRequestDto dto) : IRequest<Result>;
}
