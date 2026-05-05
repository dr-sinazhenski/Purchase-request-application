using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.ProductLogic.GetProductById;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.GetAllProducts
{
    public class GetAllProductsCommandHandler : IRequestHandler<GetAllProductsCommand, Result<List<ProductResDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllProductsCommandHandler> _logger;

        public GetAllProductsCommandHandler(AppDbContext dbContext, ILogger<GetAllProductsCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<Result<List<ProductResDto>>> Handle(GetAllProductsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all products");

            var products = await _dbContext.Products
                .AsNoTracking()
                .Select(p => new ProductResDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    RequestTypeIds = p.RequestType.Select(rt => rt.Id).ToList()
                }).ToListAsync();

            return Result<List<ProductResDto>>.Success(products);
        }
    }
}
