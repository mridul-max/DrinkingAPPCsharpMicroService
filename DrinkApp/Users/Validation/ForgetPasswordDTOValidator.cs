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
    public class ForgetPasswordDTOValidator : AbstractValidator<ForgetPasswordDTO>
    {
        public ForgetPasswordDTOValidator()
        {
            RuleFor(x => x.GenerateTokenCode).NotNull();
            RuleFor(x => x.NewPassword).NotEmpty()
                .WithMessage("Your password cannot be empty")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
