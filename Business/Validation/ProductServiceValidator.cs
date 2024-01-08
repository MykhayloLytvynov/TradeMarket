using System;
using System.Collections.Generic;
using System.Text;
using Business.Models;
using FluentValidation;

namespace Business.Validation
{
    public class ProductServiceValidator : AbstractValidator<ProductModel>
    {
        public ProductServiceValidator()
        {
            RuleFor(product => product.ProductName)
                .NotEmpty().WithMessage("Product name cannot be empty.")
                .NotNull().WithMessage("Product name cannot be null.");
            RuleFor(product => product.Price)
                .Must(BeAValidPrice).WithMessage("Invalid birthdate. Please provide a valid date.");
        }

        private bool BeAValidPrice(decimal price)
        {
            return price >= 0;
        }
    }
}
