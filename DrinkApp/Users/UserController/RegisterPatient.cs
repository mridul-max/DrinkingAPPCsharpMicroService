using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
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
        [OpenApiParameter("role",In = ParameterLocation.Query, Required = true, Type = typeof(Role), Description = "Patient role define")]
        [OpenApiParameter("status", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "active status of the patient ")]
        [OpenApiOperation(operationId: "RegisterPatient", tags: new[] { "Users" }, Summary = "Register a new user as a patient")]
        [OpenApiRequestBody("application/json", typeof(RegisterPatientDTO), Description = "Registers a new User as a patient.", Example = typeof(RegisterPatientDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(PatientResponseDTO), Description = "The OK response with the new user.")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register/patient")] HttpRequestData req, string role, bool status)
        {
            _logger.LogInformation("Creating new user.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                RegisterPatientDTOValidator validator = new RegisterPatientDTOValidator();
                var data = JsonConvert.DeserializeObject<RegisterPatientDTO>(requestBody);
                var validationResult = validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
                var result = await _patientService.RegisteredPatientAsync(data, role, status);
                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(result);
                return new CreatedAtActionResult("Add Patient", "RegisterPatient.cs", "none", result);
            }
            catch (RegisterUserExistingException ex)
            {
                return new UserCreationConflictObjectResult(ex.Message);
            }
        }
    }
}

