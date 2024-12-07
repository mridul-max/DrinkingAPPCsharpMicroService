using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
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
    public class GetPatient
    {
        private ILogger Logger { get; }
        private IPatientService _patientService { get; }

        public GetPatient(ILogger<GetPatient> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("GetPatient")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "GetPatientById",
            tags: new[] { "Patients" },
            Summary = "Retrieve a Patient by ID",
            Description = "Fetches a single patient by their unique identifier. Requires Admin or Caregiver role."
        )]
        [OpenApiParameter(
            name: "id",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(Guid),
            Description = "The unique identifier of the patient."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PatientResponseDTO),
            Description = "Successfully retrieved the patient."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.NotFound,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Patient not found."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Unauthorized,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Unauthorized access."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Forbidden,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Access forbidden for the current user."
        )]
        public async Task<HttpResponseData> GetPatientById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients/{id:guid}")] HttpRequestData req,
            Guid id,
            FunctionContext context)
        {
            ClaimsPrincipal claimsPrincipal = context.GetUser();
            if (claimsPrincipal == null)
            {
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Unauthorized, new
                {
                    Error = "Unauthorized",
                    Details = "You must be logged in to access this resource."
                });
            }

            try
            {
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString()) || claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
                {
                    var patient = await _patientService.GetOnePatient(id);
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, patient);
                }
                else
                {
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Forbidden, new
                    {
                        Error = "Forbidden",
                        Details = "You do not have permission to access this resource."
                    });
                }
            }
            catch (NotFoundException ex)
            {
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.NotFound, new
                {
                    Error = "Not Found",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.InternalServerError, new
                {
                    Error = "Internal Server Error",
                    Details = "An error occurred while processing your request."
                });
            }
        }
    }
}
