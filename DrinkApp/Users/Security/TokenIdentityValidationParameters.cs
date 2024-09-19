using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Security
{
    public class TokenIdentityValidationParameters : TokenValidationParameters
    {
        public TokenIdentityValidationParameters(string issuer, string audience, SymmetricSecurityKey securityKey)
        {
            RequireSignedTokens = true;
            ValidAudience = audience;
            ValidateAudience = true;
            ValidIssuer = issuer;
            ValidateIssuer = true;
            ValidateIssuerSigningKey = true;
            ValidateLifetime = true;
            IssuerSigningKey = securityKey;
            AuthenticationType = "Bearer";
        }
    }
}
