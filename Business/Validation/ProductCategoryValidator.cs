using System;
using System.Collections.Generic;
using System.Text;
using Business.Models;
using FluentValidation;

namespace Business.Validation
{
    public class ProductCategoryValidator : AbstractValidator<ProductCategoryModel>
    {
        public ProductCategoryValidator()
        {
            RuleFor(productCategory => productCategory.CategoryName)
               .NotEmpty().WithMessage("Product name cannot be empty.")
               .NotNull().WithMessage("Product name cannot be null.");
        }
    }
}
