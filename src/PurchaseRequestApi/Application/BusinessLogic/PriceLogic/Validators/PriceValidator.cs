using FluentValidation;
using Application.BusinessLogic.PriceLogic.Dto;

namespace Application.BusinessLogic.PriceLogic.Validators
{
    public class PriceValidator : AbstractValidator<CrudPriceDto>
    {
        public PriceValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required");

            RuleFor(x => x.RegionId)
                .NotEmpty().WithMessage("RegionId is required");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.UnitsOfMeasure)
                .NotEmpty().WithMessage("Units of measure is required")
                .MaximumLength(20).WithMessage("Units of measure must not exceed 20 characters");
        }
    }
}