using FluentValidation;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Validators
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(1, 100)
                .WithMessage("Name must be between 1 and 100 characters");

            RuleFor(x => x.Brand)
                .NotEmpty()
                .WithMessage("Brand is required")
                .Length(1, 100)
                .WithMessage("Name must be between 1 and 100 characters");

            RuleFor(x => x.Price)
               .GreaterThan(0)
               .WithMessage("Price must be greater than 0")
               .LessThan(1000000)
               .WithMessage("Price must be less than 1,000,000");
        }
    }
}
