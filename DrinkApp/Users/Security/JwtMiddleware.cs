using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Security.TokenServices;

namespace Users.Security
{
    public class JwtMiddleware : IFunctionsWorkerMiddleware
    {
        ITokenService _tokenService { get; }
        ILogger Logger { get; }
        public JwtMiddleware(ITokenService TokenService, ILogger<JwtMiddleware> logger)
        {
            this._tokenService = TokenService;
            this.Logger = Logger;
        }

        public async Task Invoke(FunctionContext Context, FunctionExecutionDelegate Next)
        {
            string HeadersString = (string)Context.BindingContext.BindingData["Headers"];
            Dictionary<string, string> headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(HeadersString);

            if(headers.TryGetValue("Authorization", out string authHeader))
            {
                try
                {
                    AuthenticationHeaderValue bearerHeader = AuthenticationHeaderValue.Parse(authHeader);
                    ClaimsPrincipal User = await _tokenService.GetByValue(bearerHeader.Parameter);
                    Context.Items["User"] = User;
                }
                catch(Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }
            await Next(Context);
        }
    }
}
