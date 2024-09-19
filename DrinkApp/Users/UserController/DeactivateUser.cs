using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
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
using Users.Security;
using Users.Services;

namespace Users.UserController
{
    public class DeactivateUser
    {
        private ILogger Logger { get; }
        private IPatientService _patientService { get; }

        public DeactivateUser(ILogger<EditDailyLimit> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("DeactivateUser")]
        [UsersAuth]
        [OpenApiOperation(operationId: "deactivate", tags: new[] { "Users" }, Summary = "Deactivates a user")]     
        [OpenApiParameter("userId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "id of the user to set status")]
        [OpenApiParameter("status", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "status of the user account")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Patient), Description = "the created response with the edited user")]
        public async Task<IActionResult> Deactivate([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/deactivateuser")] HttpRequestData req, Guid userId, bool status, FunctionContext Context)
        {
            Logger.LogInformation("Deactivate the patient");
            ClaimsPrincipal claimsPrincipal = Context.GetUser();
            if (claimsPrincipal == null)
            {
                return new UnauthorizedResult();
            }
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseData response = req.CreateResponse();
            try
            {
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString()))
                {
                    bool user = await _patientService.DeactivateUser(userId, status);
                    return new CreatedAtActionResult("Deactivate users", "DeactivateUser.cs", "none", response.ToString());
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
