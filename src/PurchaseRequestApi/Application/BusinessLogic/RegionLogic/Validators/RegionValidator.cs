using FluentValidation;
using Application.BusinessLogic.RegionLogic.Dto;

namespace Application.BusinessLogic.RegionLogic.Validators
{
    public class RegionValidator : AbstractValidator<CrudRegionDto>
    {
        public RegionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be a 3-character ISO code (e.g. USD, EUR)");
        }
    }
}