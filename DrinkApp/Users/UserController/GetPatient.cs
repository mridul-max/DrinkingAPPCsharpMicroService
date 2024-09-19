using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
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
        [OpenApiOperation(operationId: "Gets Patient", tags: new[] { "Users" }, Summary = "Gets Patient by Id")]
        [OpenApiParameter("Guid", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "Id of the Patient")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PatientResponseDTO), Description = "The OK response with a Patient")]
        public async Task<IActionResult> RunDailyGoalCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patient/getpatient/{Guid}")] HttpRequestData req, Guid Guid, FunctionContext Context)
        {
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                return new UnauthorizedResult();
            }
            try
            {
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString())|| claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
                {
                    var response = await _patientService.GetOnePatient(Guid);
                    return new CreatedAtActionResult("Get Patient", "GetPatient.cs", "none", response);
                }
                else
                {
                    return new ForbidResult(HttpStatusCode.Forbidden.ToString());
                }             
            }
            catch (NotFoundException e)
            {
                return new EntryNotFoundObjectResult(e.Message);
            }
        }
    }
}
