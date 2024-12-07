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
    public class GetCareGiver
    {
        private readonly ILogger<GetCareGiver> _logger;
        private readonly ICareGiverService _careGiverService;

        public GetCareGiver(ILogger<GetCareGiver> logger, ICareGiverService careGiverService)
        {
            _logger = logger;
            _careGiverService = careGiverService;
        }

        [Function("GetCareGiver")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "GetCareGiverById",
            tags: new[] { "CareGivers" },
            Summary = "Retrieve a caregiver by their ID.",
            Description = "Returns details of a caregiver if the requesting user is authorized."
        )]
        [OpenApiParameter(
            name: "caregiverId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(Guid),
            Description = "The unique identifier of the caregiver."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(CareGiverResponseDTO),
            Description = "Details of the requested caregiver."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.NotFound,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Caregiver not found."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Forbidden,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Access denied for the current user."
        )]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "caregivers/{caregiverId}")] HttpRequestData req,
            Guid caregiverId,
            FunctionContext context)
        {
            ClaimsPrincipal claimsPrincipal = context.GetUser();
            if (claimsPrincipal == null)
            {
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Unauthorized, new
                {
                    Error = "Unauthorized",
                    Details = "Authentication is required."
                });
            }

            _logger.LogInformation("Retrieving caregiver details for ID: {CaregiverId}", caregiverId);

            try
            {
                // Authorization check
                if (claimsPrincipal.IsInRole(Role.ADMIN.ToString()) || claimsPrincipal.IsInRole(Role.CARE_GIVER.ToString()))
                {
                    var caregiver = await _careGiverService.GetOneCareGiver(caregiverId);
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, caregiver);
                }
                else
                {
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Forbidden, new
                    {
                        Error = "Forbidden",
                        Details = "You do not have access to this resource."
                    });
                }
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Caregiver not found: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.NotFound, new
                {
                    Error = "Not Found",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occurred: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.InternalServerError, new
                {
                    Error = "Internal Server Error",
                    Details = "An error occurred while processing the request."
                });
            }
        }
    }
}
