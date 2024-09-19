using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Security
{
    public class LoginResult
    {
        private JwtSecurityToken Token { get; }
        [OpenApiProperty(Description = "The access token to be used in every subsequent operation for this user.")]
        [JsonRequired]
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(Token);

        [OpenApiProperty(Description = "The token type.")]
        [JsonRequired]
        public string TokenType => "Bearer";

        [OpenApiProperty(Description = "The amount of seconds until the token expires")]
        [JsonRequired]
        public int ExpiresIn => (int)(Token.ValidTo - DateTime.UtcNow).TotalSeconds;

        public LoginResult(JwtSecurityToken token)
        {
            Token = token;
        }
    }
}
