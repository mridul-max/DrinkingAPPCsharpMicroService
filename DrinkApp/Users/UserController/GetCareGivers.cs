using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Model.DTO.RespononseDTO;
using Users.Model.CustomException;
using Users.Security;
using Users.Services;
using Users.Model;

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

        [Function("GetAllCareGivers")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "GetAllCareGivers",
            tags: new[] { "CareGivers" },
            Summary = "Retrieve all Caregivers",
            Description = "Fetches the list of all caregivers. Requires Admin or Caregiver role."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<CareGiverResponseDTO>),
            Description = "The list of caregivers."
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
            Description = "No caregivers found."
        )]
        public async Task<HttpResponseData> GetAllCareGivers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "caregivers")] HttpRequestData req,
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
                    var caregivers = await _careGiverService.GetAllCareGivers();
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, caregivers);
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
