using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.UpdateRequest
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly ILogger<UpdateRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdateRequestHandler(AppDbContext dbContext, ILogger<UpdateRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating request");

            var request = await _dbContext.Requests.Include(x => x.RequesterProducts).FirstOrDefaultAsync(x => x.Id == command.dto.Id);

            if (request == null)
            {
                _logger.LogInformation("Reqest not found");
                return Result<GetRequestDetailsResDto>.Failure(null);
            }

            var type = await _dbContext.RequestTypes.FirstOrDefaultAsync(x => x.Id == command.dto.RequestTypeId);
            if (type == null)
            {
                _logger.LogInformation("Reqest type not found");
                return Result<GetRequestDetailsResDto>.Failure(null);
            }

            if (request.Status != RequestStatus.Submited && request.Status != RequestStatus.ForRevision && request.Status != RequestStatus.Resubmited)
            {
                _logger.LogInformation("Reqest type incorrect");
                return Result<GetRequestDetailsResDto>.Failure(null);
            }

            request.Title = command.dto.Title;
            request.Description = command.dto.Description;
            request.RequestTypeId = type.Id;
            request.RequestType = type;
            request.UpdatedAt = DateTime.UtcNow;


            var productsIdsAmount = command.dto.ProductIdAmount.Where(x => x.Value > 0).ToDictionary();
            var productsIds = productsIdsAmount.Keys.ToList();

            var toUpdate = request.RequesterProducts.Where(rp => productsIds.Contains(rp.ProductId)).ToList();
            var toDelete = request.RequesterProducts.Where(rp => !productsIds.Contains(rp.ProductId)).ToList();
            var toInsert = productsIds.Where(id => !toUpdate.Any(pr => pr.ProductId == id)).ToList();

            foreach (var rp in toUpdate)
            {
                rp.Quantity = productsIdsAmount[rp.ProductId];
            }

            foreach (var rp in toDelete)
            {
                request.RequesterProducts.Remove(rp);
            }

            var products = _dbContext.Products.Where(p => toInsert.Contains(p.Id) && p.RequestType.Contains(type)).ToList();

            RequesterProduct requesterProduct;
            foreach (var product in products)
            {
                if (command.dto.ProductIdAmount[product.Id] > 0)
                {
                    requesterProduct = new RequesterProduct
                    {
                        Request = request,
                        RequestId = request.Id,
                        Product = product,
                        ProductId = product.Id,
                        Quantity = command.dto.ProductIdAmount[product.Id]
                    };

                    request.RequesterProducts.Add(requesterProduct);

                    await _dbContext.RequesterProducts.AddAsync(requesterProduct);
                }
            }

            if (request.Status is RequestStatus.ForRevision)
            {
                request.Status = RequestStatus.Resubmited;
            }

            _dbContext.Requests.Update(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Reqest updated");

            var reqDto = new GetRequestDetailsResDto()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                RequestType = new RequestTypeResDto()
                {
                    Id = request.RequestType.Id,
                    Name = request.RequestType.Name,
                },
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Products = new List<ProductListItemDto>()
            };

            var productsForDto = await _dbContext.Products.Include(x => x.RequestType).Where(x => productsIds.Contains(x.Id)).ToListAsync();

            var prices = _dbContext.Prices.Where(p => p.RegionId == new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") && productsIds.Contains(p.Product.Id)).ToList();

            foreach (var product in productsForDto)
            {
                reqDto.Products.Add(new ProductListItemDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Amount = command.dto.ProductIdAmount[product.Id],
                    Price = prices.First(p => p.ProductId == product.Id).Amount
                });
            }

            if (request.RejectionComment != null)
            {
                reqDto.RejectionCommentText = request.RejectionComment.ToString();
            }

            return Result<GetRequestDetailsResDto>.Success(reqDto);
        }
    }
}
