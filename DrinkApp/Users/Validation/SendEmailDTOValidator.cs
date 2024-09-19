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
    public class SendEmailDTOValidator : AbstractValidator<SendEmailDTO>
    {
        public SendEmailDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required")
                     .EmailAddress().WithMessage("A valid email is required");
        }
    }
}
