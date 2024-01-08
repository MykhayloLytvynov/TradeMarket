using System;
using System.Collections.Generic;
using System.Text;
using Business.Models;
using FluentValidation;

namespace Business.Validation
{
    public class CustomerServiceValidator : AbstractValidator<CustomerModel>
    {
        public CustomerServiceValidator()
        {
            RuleFor(customer => customer.Name)
                .NotEmpty().WithMessage("Customer name cannot be empty.")
                .NotNull().WithMessage("Customer name cannot be null.");
            RuleFor(customer => customer.Surname)
                .NotEmpty().WithMessage("Customer surname cannot be empty.")
                .NotNull().WithMessage("Customer surname cannot be null.");
            RuleFor(customer => customer.BirthDate)
            .Must(BeAValidDate).WithMessage("Invalid birthdate. Please provide a valid date.");
            RuleFor(customer => customer.DiscountValue)
            .Must(BeAValidDateDiscountValue).WithMessage("Invalid discountValue. Please provide a valid date.");
        }

        private bool BeAValidDate(DateTime birthDate)
        {
            return birthDate > DateTime.Now.AddYears(-100) && birthDate < DateTime.Now;
        }

        private bool BeAValidDateDiscountValue(int discountValue)
        {
            return discountValue > 0;
        }
    }
}
