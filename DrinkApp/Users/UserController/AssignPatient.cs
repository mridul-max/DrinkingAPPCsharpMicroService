using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Users.Model.DTO;
using Users.Services;
using Microsoft.OpenApi.Models;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Users.Model.CustomException;
using Users.Model;
using System.Security.Claims;
using Users.Security;
using Users.Model.DTO.RespononseDTO;

namespace Users.UserController
{
    public class AssignPatient
    {
        private ILogger Logger { get; }
        private ICareGiverService _careGiverService { get; }
        public AssignPatient(ILogger<AssignPatient> log, ICareGiverService careGiverService)
        {
            Logger = log;
            _careGiverService = careGiverService;
        }

        [Function("AssignPatientToCareGiver")]
        [UsersAuth]
        [OpenApiOperation(operationId: "AssignPatient", tags: new[] { "Users" }, Summary = "Assigns a patient to the list of a specific care giver")]
        [OpenApiParameter("caregiverId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The Guid of the caregiver")]
        [OpenApiParameter("patientId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The Guid of the Patient to assign Caregiver")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CareGiverResponseDTO), Description = "The OK response")]
        public async Task<IActionResult> Assign(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "patient/assigncaregiver")] HttpRequestData req, Guid caregiverId,Guid patientId, FunctionContext Context)
        {
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                return new UnauthorizedResult();
            }
            Logger.LogInformation("Assigning care giver to patient.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseData response = req.CreateResponse();
            try
            {
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString()) || claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
                {
                    await _careGiverService.AssignPatient(caregiverId, patientId);
                    var caregiver = _careGiverService.GetOneCareGiver(caregiverId);
                    return new CreatedAtActionResult("AssignPatient", "AssignPatient.cs", "none", caregiver);
                }
                else
                {
                    return new ForbidResult(HttpStatusCode.Forbidden.ToString());
                }
            }
            catch (NotFoundException ex)
            {
                return new EntryNotFoundObjectResult(ex.Message);
            }
        }
    }
}
