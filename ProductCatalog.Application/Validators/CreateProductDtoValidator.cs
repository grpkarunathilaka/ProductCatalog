using FluentValidation;
using ProductCatalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
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
               .LessThan(decimal.MaxValue)
               .WithMessage("Price must be less than 1,000,000");
        }
    }
}
