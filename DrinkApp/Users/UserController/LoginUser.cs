using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Users.Model.DTO;
using Users.Security.TokenServices;
using Users.Security;
using Users.Model.CustomException;

namespace Users.UserController
{
    public class LoginUser
    {
        private ILogger Logger { get; }
        private ITokenService TokenService { get; }

        public LoginUser(ILogger<LoginUser> log, ITokenService tokenService)
        {
            Logger = log;
            TokenService = tokenService;
        }

        [Function("Login")]
        [OpenApiOperation(operationId: "LoginUser", tags: new[] { "Users" }, Summary = "A user login with username and password")]
        [OpenApiRequestBody("application/json", typeof(LoginRequest), Description = "Login a user.", Example = typeof(LoginDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object), Description = "User not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "Login success")]
        public async Task<HttpResponseData> Login([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
        {
            Logger.LogInformation("Log in is called.");
            try
            {
                // Deserialize the request body
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                LoginRequest login = JsonConvert.DeserializeObject<LoginRequest>(requestBody);

                // Generate the token
                var result = await TokenService.CreateToken(login);

                // Create a successful response
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(result);
                return response;
            }
            catch (NotFoundException ex)
            {
                Logger.LogWarning("Login failed: {Message}", ex.Message);

                // Create a response with a 404 status code
                var response = req.CreateResponse();

                // Create an error object with the exception message
                var errorResponse = new
                {
                    Error = "User not found",
                    Details = ex.Message
                };

                // Write the error object as JSON to the response
                await response.WriteAsJsonAsync(errorResponse);
                response.StatusCode = HttpStatusCode.NotFound;
                // Return the response
                return response;
            }
        }
    }
}
