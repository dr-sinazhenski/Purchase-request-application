using Application.BusinessLogic.ProductLogic.CreateProduct;
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

namespace Application.BusinessLogic.RequestLogic.CreateRequest
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly ILogger<CreateRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreateRequestHandler(AppDbContext dbContext, ILogger<CreateRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new request");

            var type = _dbContext.RequestTypes.FirstOrDefault(x => x.Id == command.dto.RequestTypeId);
            if (type == null)
            {
                return Result<GetRequestDetailsResDto>.Failure(null);
            }

            var request = new Request
            {
                Id = Guid.NewGuid(),
                Title = command.dto.Title,
                Description = command.dto.Description,
                RequestTypeId = type.Id,
                RequestType = type,
                Status = RequestStatus.Submited,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RequesterProducts = new List<RequesterProduct>(),
                RequesterId = command.dto.RequesterId
            };


            var productsIds = command.dto.ProductIdAmount.Keys.ToList();
            var products = _dbContext.Products.Include(x => x.RequestType).Where(p => productsIds.Contains(p.Id) && p.RequestType.Contains(type)).ToList();

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


            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();

            var reqDto = new GetRequestDetailsResDto()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status.ToString(),
                RequestType = new RequestTypeResDto()
                {
                    Id = request.RequestType.Id,
                    Name = request.RequestType.Name,
                },
                Products = new List<ProductListItemDto>(),
                RequesterId = (Guid)request.RequesterId
            };

            var prices = _dbContext.Prices.Where(p => p.RegionId == new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") && productsIds.Contains(p.Product.Id)).ToList();

            foreach (var product in products)
            {
                if (command.dto.ProductIdAmount[product.Id] > 0)
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
            }

            _logger.LogInformation($"Request created id= {reqDto.Id}");
            return Result<GetRequestDetailsResDto>.Success(reqDto);
        }
    }
}
