using FluentValidation;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Validators
{
    public class ProductQueryDtoValidator : AbstractValidator<ProductQueryDto>
    {
        public ProductQueryDtoValidator()
        {
            RuleFor(x => x.PageNumber)
              .GreaterThan(0)
              .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
               .GreaterThan(0)
               .WithMessage("Page size must be greater than 0")
               .LessThanOrEqualTo(100)
               .WithMessage("Page size must be less than or equal to 100");

            RuleFor(x => x.MinPrice)
              .GreaterThanOrEqualTo(0)
              .WithMessage("Minimum price must be greater than or equal to 0")
              .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum price must be greater than or equal to 0")
              .When(x => x.MaxPrice.HasValue);

            RuleFor(x =>x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
                .WithMessage("Minimum price must be less than or equal to maximum price");
        }
    }
}
