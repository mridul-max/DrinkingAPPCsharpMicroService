using Microsoft.AspNetCore.Http;
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
using System.Threading.Tasks;
using Users.Model;
using Users.Model.CustomException;
using Users.Model.DTO;
using Users.Model.DTO.RespononseDTO;
using Users.Services;
using Users.Validation;

namespace Users.UserController
{
    public class RegisterPatient
    {
        private readonly ILogger<RegisterPatient> _logger;
        private readonly IPatientService _patientService;

        public RegisterPatient(ILogger<RegisterPatient> log, IPatientService patientService)
        {
            _logger = log;
            _patientService = patientService;
        }

        [Function("RegisterPatient")]
        [OpenApiParameter("role", In = ParameterLocation.Query, Required = true, Type = typeof(Role), Description = "Patient role define")]
        [OpenApiParameter("status", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "Active status of the patient")]
        [OpenApiOperation(operationId: "RegisterPatient", tags: new[] { "Patients" }, Summary = "Register a new user as a patient")]
        [OpenApiRequestBody("application/json", typeof(RegisterPatientDTO), Description = "Registers a new User as a patient.", Example = typeof(RegisterPatientDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(PatientResponseDTO), Description = "The OK response with the new user.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Description = "Validation errors.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "application/json", bodyType: typeof(object), Description = "User already exists.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Description = "Internal server error.")]
        public async Task<HttpResponseData> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "patients/register")] HttpRequestData req, string role, bool status)
        {
            _logger.LogInformation("Creating new user.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var response = req.CreateResponse();
            try
            {
                RegisterPatientDTOValidator validator = new RegisterPatientDTOValidator();
                var data = JsonConvert.DeserializeObject<RegisterPatientDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {Errors}", validationResult.Errors);
                    var errorResponse = validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    });
                    await response.WriteAsJsonAsync(errorResponse);
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                // Register the patient
                var result = await _patientService.RegisteredPatientAsync(data, role, status);               
                await response.WriteAsJsonAsync(result);
                response.StatusCode = HttpStatusCode.Created;
                return response;
            }
            catch (RegisterUserExistingException ex)
            {
                _logger.LogWarning("User registration failed: {Message}", ex.Message);
                var errorResponse = new
                {
                    Error = "User already exists",
                    Details = ex.Message
                };
                await response.WriteAsJsonAsync(errorResponse);
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }
        }
    }
}
