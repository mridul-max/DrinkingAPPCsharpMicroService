using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
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
        [OpenApiOperation(operationId: "RegisterCareGiver", tags: new[] { "CareGivers" }, Summary = "Register a new user as a CareGiver")]
        [OpenApiRequestBody("application/json", typeof(RegisterCareGiverDTO), Description = "Registers a new User as a CareGiver.", Example = typeof(RegisterCareGiverDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CareGiverResponseDTO), Description = "The OK response with the new user.", Example = typeof(RegisterCareGiverDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Validation errors.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "application/json", bodyType: typeof(object), Description = "User already exists.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Description = "Internal server error.")]
        public async Task<HttpResponseData> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "caregivers/register")] HttpRequestData req, string role, bool status, FunctionContext Context)
        {
            _logger.LogInformation("Creating new CareGiver.");
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Unauthorized, "Unauthorized, Authentication is required.");
            }
            if (!claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()) && !claimsPrincipal.IsInRole(Role.ADMIN.ToString()))
            {
                _logger.LogWarning("Forbidden access attempt by user.");
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Forbidden,"Forbidden You do not have permission to perform this action.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                RegisterCareGiverDTOValidator validator = new RegisterCareGiverDTOValidator();
                var data = JsonConvert.DeserializeObject<RegisterCareGiverDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {Errors}", validationResult.Errors);
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.BadRequest, validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
                // Process registration
                var result = await _careGiverService.RegisteredCareGiverAsync(data, role, status);

                // Return success response
                var response = req.CreateResponse();
                await response.WriteAsJsonAsync(result);
                response.StatusCode = HttpStatusCode.Created;           
                return response;
            }
            catch (RegisterUserExistingException ex)
            {
                _logger.LogWarning("Registration conflict: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.InternalServerError, "\"Internal Server Error\" An unexpected error occurred. Please try again later.");
            }
        }
    }
}
