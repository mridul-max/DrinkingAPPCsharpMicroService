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
using Users.Security;
using Users.Services;

namespace Users.UserController
{
    public class EditDailyLimit
    {
        private readonly ILogger Logger;
        private readonly IPatientService _patientService;

        public EditDailyLimit(ILogger<EditDailyLimit> log, IPatientService patientService)
        {
            Logger = log;
            _patientService = patientService;
        }

        [Function("EditDailyLimit")]
        [UsersAuth]
        [OpenApiOperation(
            operationId: "EditDailyLimit",
            tags: new[] { "Patients" },
            Summary = "Edit the daily drink limit for a patient",
            Description = "Allows an authorized user to update the daily drink limit of a patient identified by their phone number."
        )]
        [OpenApiParameter(
            name: "phoneNo",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(string),
            Description = "The phone number of the patient whose daily drink limit needs to be updated."
        )]
        [OpenApiParameter(
            name: "newLimit",
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "The new daily drink limit for the patient."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(Patient),
            Description = "Successfully updated the daily drink limit for the patient."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.Unauthorized,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "Unauthorized access."
        )]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.NotFound,
            contentType: "application/json",
            bodyType: typeof(object),
            Description = "The patient with the specified phone number was not found."
        )]
        public async Task<HttpResponseData> EditLimit(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "patients/{phoneNo}/daily-limit")] HttpRequestData req,
            string phoneNo,
            int newLimit,
            FunctionContext context)
        {
            Logger.LogInformation("Processing request to update daily drink limit for PhoneNo: {PhoneNo} to {NewLimit}", phoneNo, newLimit);

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
                // Validate new limit
                if (newLimit < 0)
                {
                    return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.BadRequest, new
                    {
                        Error = "Invalid Data",
                        Details = "The daily limit must be a non-negative integer."
                    });
                }

                // Edit the patient's daily limit
                await _patientService.EditPatientLimit(phoneNo, newLimit);

                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.OK, "Daily Limit is successfully updated");
            }
            catch (NotFoundException ex)
            {
                Logger.LogWarning("Patient with phone number {PhoneNo} not found: {Message}", phoneNo, ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.NotFound, new
                {
                    Error = "Not Found",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occurred while updating the daily drink limit: {Message}", ex.Message);
                return HttpResponseHelper.CreateResponseData(req, HttpStatusCode.InternalServerError, new
                {
                    Error = "Internal Server Error",
                    Details = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
