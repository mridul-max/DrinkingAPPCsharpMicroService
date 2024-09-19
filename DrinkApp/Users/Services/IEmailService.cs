using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model.DTO;

namespace Users.Services
{
    public interface IEmailService
    {
        Task SendEmail(SendGridMessage Body);
        string GenerateTokenCode();
        Task SendEmailToResetPassword(SendEmailDTO sendEmailDTO, string phoneNumber);
    }
}
