using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.ApproveRequest
{
    public record ApproveRequestCommand(Guid id) : IRequest<Result>;
}
