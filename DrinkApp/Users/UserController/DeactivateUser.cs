using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Model.CustomException;
using Users.Security;
using Users.Services;
using Users.Model.DTO;
using Microsoft.OpenApi.Models;
using Users.Model;

namespace Users.UserController
{
    public class DeactivateUser
    {
        private readonly ILogger Logger;
        private readonly IPatientService _patientService;

        public DeactivateUser(ILogger<DeactivateUser> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("DeactivateUser")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "DeactivateUser",
            tags: new[] { "Users" },
            Summary = "Deactivate or activate a user account",
            Description = "Allows an admin to change the status of a user's account."
        )]
        [OpenApiParameter(
            name: "userId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(Guid),
            Description = "The unique identifier of the user whose account status is being changed."
        )]
        [OpenApiParameter(
            name: "status",
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(bool),
            Description = "The new status for the user's account (true for active, false for deactivated)."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Confirmation of the user's status change."
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
            Description = "User not found."
        )]
        public async Task<HttpResponseData> Deactivate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{userId}/deactivate")] HttpRequestData req,
            Guid userId,
            bool status,
            FunctionContext context)
        {
            Logger.LogInformation("Processing user account status change request for User ID: {UserId}, Status: {Status}", userId, status);

            ClaimsPrincipal claimsPrincipal = context.GetUser();
            if (claimsPrincipal == null)
            {
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Unauthorized, new
                {
                    Error = "Unauthorized",
                    Details = "You must be logged in to perform this action."
                });
            }

            try
            {
                if (!claimsPrincipal.IsInRole(Role.ADMIN.ToString()))
                {
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.Forbidden, new
                    {
                        Error = "Forbidden",
                        Details = "Only administrators can perform this action."
                    });
                }

                // Perform the deactivation or activation
                await _patientService.DeactivateUser(userId, status);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, new
                {
                    Message = "User with ID "+ userId + " has been successfully deactivated"
                });
            }
            catch (NotFoundException ex)
            {
                Logger.LogWarning("User with ID {UserId} not found: {Message}", userId, ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.NotFound, new
                {
                    Error = "Not Found",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occurred while processing the request: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.InternalServerError, new
                {
                    Error = "Internal Server Error",
                    Details = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
