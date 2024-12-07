using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Model;
using Users.Model.CustomException;
using Users.Model.DTO.RespononseDTO;
using Users.Security;
using Users.Services;

namespace Users.UserController
{
    public class AssignPatient
    {
        private readonly ILogger<AssignPatient> _logger;
        private readonly ICareGiverService _careGiverService;

        public AssignPatient(ILogger<AssignPatient> logger, ICareGiverService careGiverService)
        {
            _logger = logger;
            _careGiverService = careGiverService;
        }

        [Function("AssignPatientToCareGiver")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "AssignPatient",
            tags: new[] { "Users" },
            Summary = "Assigns a patient to a specific caregiver"
        )]
        [OpenApiParameter("caregiverId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the caregiver")]
        [OpenApiParameter("patientId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the patient to assign to the caregiver")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Created,
            contentType: "application/json",
            bodyType: typeof(CareGiverResponseDTO),
            Description = "Patient successfully assigned to caregiver"
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.NotFound,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Caregiver or patient not found"
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Unauthorized,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Unauthorized"
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Forbidden,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Forbidden"
        )]
        public async Task<HttpResponseData> Assign(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "caregivers/{caregiverId}/patients/{patientId}/assign")] HttpRequestData req,
            Guid caregiverId,
            Guid patientId,
            FunctionContext context)
        {
            _logger.LogInformation("Starting patient assignment to caregiver.");

            var claimsPrincipal = context.GetUser();
            if (claimsPrincipal == null)
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return CreateResponse(req, HttpStatusCode.Unauthorized, "Unauthorized access.");
            }

            if (!claimsPrincipal.IsInRole(Role.ADMIN.ToString()) && !claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
            {
                _logger.LogWarning("Forbidden access attempt by user.");
                return CreateResponse(req, HttpStatusCode.Forbidden, "You do not have permission to perform this action.");
            }

            try
            {
                // Assign patient to caregiver
                await _careGiverService.AssignPatient(caregiverId, patientId);
                var caregiver = await _careGiverService.GetOneCareGiver(caregiverId);

                // Return success response
                var response = req.CreateResponse();
                await response.WriteAsJsonAsync("A patients successfully asigned to a caregiver");
                response.StatusCode = HttpStatusCode.Created;    
                _logger.LogInformation("Patient successfully assigned to caregiver.");
                return response;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Entity not found: {Message}", ex.Message);
                return CreateResponse(req, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                return CreateResponse(req, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }
    
        private HttpResponseData CreateResponse(HttpRequestData req, HttpStatusCode statusCode, string message)
        {
            var response = req.CreateResponse();
            var errorResponse = new { Message = message };
            response.WriteAsJsonAsync(errorResponse);
            response.StatusCode = statusCode;
            return response;
        }
    }
}
