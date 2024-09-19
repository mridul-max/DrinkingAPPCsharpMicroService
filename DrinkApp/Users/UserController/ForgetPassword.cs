using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Users.Services;
using Users.Model.DTO;
using Users.Model.DTO.RespononseDTO;
using Users.Validation;
using System.Security.Claims;
using Users.Model;
using Users.Security;
using Users.Model.CustomException;

namespace Users.UserController
{
    public class ForgetPassword
    {
        private ILogger _logger { get; }
        private IPasswordHashService _passwordHashService;
        public ForgetPassword(ILogger<ForgetPassword> Log, IPasswordHashService passwordHashService)
        {
            _logger = Log;
            _passwordHashService = passwordHashService;
        }

        [Function("ForgetPassword")]
        [OpenApiOperation(operationId: "ForgetPassword", tags: new[] { "Users" }, Summary = "Update user password by having password token.")]
        [OpenApiParameter("PhoneNumber", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "phone number of the user to update password")]
        [OpenApiRequestBody("application/json", typeof(ForgetPasswordDTO), Description = "Update user password by having password token", Example = typeof(ForgetPasswordDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ForgetPasswordResponseDTO), Description = "To update user password by having password token.")]
        public async Task<IActionResult> ForgetPass([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "resetpassword")] HttpRequestData req, string PhoneNumber)
        {
            _logger.LogInformation("C# HTTP trigger reset password processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                ForgetPasswordDTOValidator validator = new ForgetPasswordDTOValidator();
                var data = JsonConvert.DeserializeObject<ForgetPasswordDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
                await _passwordHashService.PasswordResetByTokenCode(data, PhoneNumber);
                return new CreatedAtActionResult("Reset password", "ForgetPassword.cs", "none", new ForgetPasswordResponseDTO("Your password has reset successfully").ToString());
            }
            catch (NotFoundException e)
            {
                return new EntryNotFoundObjectResult(e.Message);
            }
        }

    }
}
