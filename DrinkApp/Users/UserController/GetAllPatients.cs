using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public class GetAllPatients
    {
        private ILogger Logger { get; }
        private IPatientService _patientService { get; }

        public GetAllPatients(ILogger<GetAllPatients> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("GetAllPatients")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "GetAllPatients",
            tags: new[] { "Patients" },
            Summary = "Retrieve all patients",
            Description = "Fetches the list of all patients. Requires Admin or Caregiver role."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<PatientResponseDTO>),
            Description = "The list of patients."
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
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.NotFound,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "No patients found."
        )]
        public async Task<HttpResponseData> GetPatients(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients")] HttpRequestData req,
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
                    var patients = await _patientService.GetAllPatients();
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, patients);
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
