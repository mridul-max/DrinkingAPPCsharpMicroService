using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
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
    public class GetAllPatients
    {
        private ILogger Logger { get; }
        private IPatientService _patientService { get; }

        public GetAllPatients(ILogger<GetAllPatients> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("GetAllPatient")]
        [UsersAuth]
        [OpenApiOperation(operationId: "Gets All Patients", tags: new[] { "Users" }, Summary = "Gets Patients")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<PatientResponseDTO>), Description = "The OK response with list of Patients")]
        public async Task<IActionResult> RunDailyGoalCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients")] HttpRequestData req, FunctionContext Context)
        {
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                return new UnauthorizedResult();
            }
            try
            {
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString()) || claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
                {
                    var response = await _patientService.GetAllPatients();
                    return new CreatedAtActionResult("Get All Patient", "GetAllPatients.cs", "none", response);
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
