using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Users.Model.DTO;
using Users.Services;
using Users.Security;
using Users.Model;
using Microsoft.AspNetCore.Mvc;
using Users.Model.CustomException;
using System.Security.Claims;
using Users.Model.DTO.RespononseDTO;

namespace Users.UserController
{
    public class GetCareGiver
    {
        private ILogger Logger { get; }
        private ICareGiverService _careGiverService { get; }

        public GetCareGiver(ILogger<GetCareGiver> log, ICareGiverService careGiverService)
        {
            Logger = log;
            _careGiverService = careGiverService;
        }

        [Function("GetCareGiver")]
        [UsersAuth]
        [OpenApiOperation(operationId: "Gets Caregiver", tags: new[] { "Users" }, Summary = "Gets Caregiver by Id")]
        [OpenApiParameter("Guid", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "Id of the Caregiver")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CareGiverResponseDTO), Description = "The OK response with a caregiver")]
        public async Task<IActionResult> RunDailyGoalCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "caregiver/getcaregiver/{Guid}")] HttpRequestData req, Guid Guid, FunctionContext Context)
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
                    var response = await _careGiverService.GetOneCareGiver(Guid);
                    return new CreatedAtActionResult("Get Caregiver", "GetCareGiver.cs", "none", response);
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
