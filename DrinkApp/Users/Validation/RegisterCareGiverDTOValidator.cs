using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Users.Model.DTO;

namespace Users.Validation
{
    public class RegisterCareGiverDTOValidator : AbstractValidator<RegisterCareGiverDTO>
    {
        public RegisterCareGiverDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required")
                     .EmailAddress().WithMessage("A valid email is required"); 
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull()
                .WithMessage("Phone Number is required.")
                .MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
                .MaximumLength(20).WithMessage("PhoneNumber must not exceed 50 characters.")
                .Matches(new Regex("^\\+?[1-9][0-9]{7,14}$"))
                .WithMessage("PhoneNumber not valid"); 
            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("Your password cannot be empty")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
        protected bool BeAValidAge(DateTime date)
        {
            int currentYear = DateTime.Now.Year;
            int dobYear = date.Year;

            if (dobYear <= currentYear && dobYear > (currentYear - 120))
            {
                return true;
            }

            return false;
        }
    }
}
