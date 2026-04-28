using FluentValidation;
using Application.BusinessLogic.ProductLogic.Dto;

namespace Application.BusinessLogic.ProductLogic.Validators
{
    public class ProductValidator : AbstractValidator<CreateProductReqDto>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(30).WithMessage("Name must not exceed 30 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        }
    }
}