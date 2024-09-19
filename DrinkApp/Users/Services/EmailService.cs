using System;
using System.Text;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using SendGrid;
using Users.Model.DTO;
using Microsoft.Extensions.Logging;
using Users.Model;
using Users.Model.CustomException;
using Users.Repositories;

namespace Users.Services
{
    public class EmailService : IEmailService
    {
        private ILogger _logger { get; }
        private IPatientRepository _patientGenericRepository { get; }
        private ICareGiverRepository _careGiverGenericRepository { get; }
        public EmailService(ILogger<EmailService> logger, IPatientRepository patientRepository, ICareGiverRepository careGiverGenericRepository)
        {
            _logger = logger;
            _patientGenericRepository = patientRepository;
            _careGiverGenericRepository = careGiverGenericRepository;
        }
        public async Task SendEmailToResetPassword(SendEmailDTO sendEmailDTO, string phoneNumber)
        {
            var patient = await _patientGenericRepository.GetByPhoneNumber(phoneNumber);
            var careGiver = await _careGiverGenericRepository.GetByPhoneNumber(phoneNumber);
            if (patient == null && careGiver == null)
            {
                throw new NotFoundException($"this is not valid phone number {phoneNumber}");
            }
            if (patient != null && patient.Email == sendEmailDTO.Email)
            {
                await SendEmailToPatientAsync(patient, sendEmailDTO);
            }
            else if (careGiver != null && careGiver.Email == sendEmailDTO.Email)
            {
                await SendEmailToCareGiverAsync(careGiver, sendEmailDTO);
            }
            else
            {
                throw new NotFoundException("Your email address is not found");
            }
        }

        //using send grid to send email
        string Message = "You can use this code to reset your password. ";
        public async Task SendEmailToCareGiverAsync(CareGiver careGiver, SendEmailDTO sendEmailDTO)
        {
            string generateTokenCode = GenerateTokenCode();
            string body = $"{Message}  {generateTokenCode}";
            careGiver.TokenCodeGeneratedTime = DateTime.Now;
            careGiver.GenerateTokenCode = generateTokenCode;
            await SendEmail(SendGridBuildMessage(body, sendEmailDTO.Email));
            _careGiverGenericRepository.Update(careGiver);
        }
        public async Task SendEmailToPatientAsync(Patient patient, SendEmailDTO sendEmailDTO)
        {
            string generateTokenCode = GenerateTokenCode();
            string body = $"{Message}  {generateTokenCode}";
            patient.TokenCodeGeneratedTime = DateTime.Now;
            patient.GenerateTokenCode = generateTokenCode;
            await SendEmail(SendGridBuildMessage(body, sendEmailDTO.Email));
            _patientGenericRepository.Update(patient);
        }
        public SendGridMessage SendGridBuildMessage(string body, string sendEmailDTO)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Environment.GetEnvironmentVariable("SENDER_ADDRESS"), "Omring"),
                Subject = "Forget Passowrd Token",
                PlainTextContent = "",
                HtmlContent = $"<strong>{body} .your code will expire soon</strong>"
            };
            msg.AddTo(new EmailAddress(sendEmailDTO, null));
            return msg;
        }
        public async Task SendEmail(SendGridMessage msg)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
        private readonly Random random = new Random();
        public string RandomGenerateCode(int size,bool lowerCase = false)
        {
            var builder = new StringBuilder(size);
            Random random = new Random();
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;
            for (var i = 0; i < size; i++)
            {
                var @char = (char)random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        } 
        public string GenerateTokenCode()
        {
            var passwordBuilder = new StringBuilder(); 
            passwordBuilder.Append(RandomGenerateCode(6, true));
            passwordBuilder.Append(RandomNumber(1000, 9999));
            passwordBuilder.Append(RandomGenerateCode(2));
            return passwordBuilder.ToString();
        }
    }
}
