using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [OpenApiRequestBody("application/json", typeof(LoginRequest), Description = "login a User.", Example = typeof(LoginDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "Login success")]
        public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
        {
            Logger.LogInformation("log in is called.");
            try
            {
                LoginRequest login = JsonConvert.DeserializeObject<LoginRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                var result = await TokenService.CreateToken(login);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(result);
                return new CreatedAtActionResult("login user","Login.cs",null, result);
            }
            catch (NotFoundException ex)
            {
                return new EntryNotFoundObjectResult(ex.Message);
            }
     
        }
    }
}
