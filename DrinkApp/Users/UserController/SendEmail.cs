using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Users.Model.DTO;
using Users.Services;
using Users.Validation;
using Users.Model.CustomException;
using Microsoft.OpenApi.Models;

namespace Users.UserController
{
    public class SendEmail
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IEmailService _emailService;

        public SendEmail(ILogger<SendEmail> log, IEmailService emailService)
        {
            _logger = log;
            _emailService = emailService;
        }

        [Function("SendEmail")]
        [OpenApiParameter("phoneNumber", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "phone number of the login user user")]
        [OpenApiOperation(operationId: "SendEmail", tags: new[] { "Users" }, Summary = "Send forget password code to user")]
        [OpenApiRequestBody("application/json", typeof(SendEmailDTO), Description = "Send forget password code to user.", Example = typeof(SendEmailDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Accepted, contentType: "application/json", bodyType: typeof(SendEmailDTO), Description = "The OK response with ")]
        public async Task<IActionResult> Sendemail(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{phoneNumber}/sendemail")] HttpRequestData req, string phoneNumber)
        {
            _logger.LogInformation("giving email address");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                SendEmailDTOValidator validator = new SendEmailDTOValidator();
                var data = JsonConvert.DeserializeObject<SendEmailDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
                await _emailService.SendEmailToResetPassword(data, phoneNumber);
                return new SuccessMessageResponse("we have send you password reset code");

            }
            catch (NotFoundException ex)
            {
                return new EntryNotFoundObjectResult(ex.Message);
            }
        }
    }
}
