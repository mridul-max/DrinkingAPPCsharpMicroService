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
using Users.Model;
using Users.Services;
using Users.Model.DTO;
using Users.Model.CustomException;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Users.Security;
using Users.Model.DTO.RespononseDTO;

namespace Users.UserController
{
    public class GetCareGivers
    {
        private ILogger Logger { get; }
        private ICareGiverService _careGiverService { get; }
        public GetCareGivers(ILogger<GetCareGivers> log, ICareGiverService careGiverService)
        {
            Logger = log;
            _careGiverService = careGiverService;
        }

        [Function("GetPatients")]
        [UsersAuth]
        [OpenApiOperation(operationId: "GetAllCareGivers", tags: new[] { "Users" }, Summary = "Gets all the care givers")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<CareGiverResponseDTO>), Description = "The OK response with the list of Care givers")]
        public async Task<IActionResult> GetAllCareGivers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "caregivers")] HttpRequestData req, FunctionContext Context)
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
                    var careGivers = await _careGiverService.GetAllCareGivers();
                    return new CreatedAtActionResult("Get All Care Givers", "GetCareGivers.cs", "none", careGivers);
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
