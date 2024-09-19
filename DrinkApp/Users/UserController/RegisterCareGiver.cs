using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Model;
using Users.Model.CustomException;
using Users.Model.DTO;
using Users.Model.DTO.RespononseDTO;
using Users.Security;
using Users.Services;
using Users.Validation;

namespace Users.UserController
{
    public class RegisterCareGiver
    {
        private readonly ILogger<RegisterCareGiver> _logger;
        private readonly ICareGiverService _careGiverService;
        public RegisterCareGiver(ILogger<RegisterCareGiver> log, ICareGiverService careGiverService)
        {
            _logger = log;
            _careGiverService = careGiverService;
        }


        [Function("RegisterCareGiver")]
        [UsersAuth]
        [OpenApiParameter("role", In = ParameterLocation.Query, Required = true, Type = typeof(Role), Description = "caregiver role define")]
        [OpenApiParameter("status", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "active status of the caregiver ")]
        [OpenApiOperation(operationId: "RegisterCareGiver", tags: new[] { "Users" }, Summary = "Register a new user as a CareGiver")]
        [OpenApiRequestBody("application/json", typeof(RegisterCareGiverDTO), Description = "Registers a new User as a CareGiver.", Example = typeof(RegisterCareGiverDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CareGiverResponseDTO), Description = "The OK response with the new user.", Example = typeof(RegisterCareGiverDTOExampleGenerator))]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register/caregiver")] HttpRequestData req, string role, bool status, FunctionContext Context)
        {
            _logger.LogInformation("Creating new CareGiver.");
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                return new UnauthorizedResult();
            }
            if (!claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()) && !claimsPrincipal.IsInRole(Role.ADMIN.ToString()))
            {
                return new ForbidResult(HttpStatusCode.Forbidden.ToString());
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                RegisterCareGiverDTOValidator validator = new RegisterCareGiverDTOValidator();
                var data = JsonConvert.DeserializeObject<RegisterCareGiverDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
                var response = await _careGiverService.RegisteredCareGiverAsync(data, role, status);
                return new CreatedAtActionResult("Add CareGiver", "RegisterCareGiver.cs", "register/careGiver", response);
            }
            catch (RegisterUserExistingException ex)
            {
                return new UserCreationConflictObjectResult(ex.Message);
            }
        }
    }
}
