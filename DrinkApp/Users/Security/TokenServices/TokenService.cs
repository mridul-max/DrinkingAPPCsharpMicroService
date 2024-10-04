using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Users.Model;
using Users.Services;

namespace Users.Security.TokenServices
{
    public class TokenService : ITokenService
    {
        private ILogger Logger { get; }
        private string Issuer { get; }
        private string Audience { get; }
        private TimeSpan ValidityDuration { get; }
        private SigningCredentials Credentials { get; }
        private TokenIdentityValidationParameters ValidationParameters { get; }
        ILoginService _loginService { get; }   

        public TokenService(IConfiguration config, ILogger<TokenService> log, ILoginService loginService)
        {
            this.Logger = log;
            _loginService = loginService;
            Issuer = config.GetValue<string>("JwtSettings:Issuer");
            Audience = config.GetValue<string>("JwtSettings:Audience");
            ValidityDuration = TimeSpan.FromDays(1);
            string key = config.GetValue<string>("JwtSettings:Key");

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, securityKey);
        }

        public async Task<LoginResult> CreateToken(LoginRequest Login)
        {
            List<UserRole> UserRoles = await _loginService.GetLoginRole(Login);
            var claims = new List<Claim>();
            // Add roles as multiple claims
            foreach (var role in UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.ToString()));
            }
            // Optionally add other app specific claims as needed
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Login.PhoneNumber));
            JwtSecurityToken Token = await CreateToken(claims.ToArray());
            return new LoginResult(Token);
        }

        public async Task<ClaimsPrincipal> GetByValue(string value)
        {
            if(value == null)
            {
                throw new Exception("No Token supplied");
            }
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken ValidatedToken;
                ClaimsPrincipal principal = handler.ValidateToken(value, ValidationParameters, out ValidatedToken);
                return await Task.FromResult(principal);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<JwtSecurityToken> CreateToken(Claim[] Claims)
        {
            JwtHeader Header = new JwtHeader(Credentials);

            JwtPayload Payload = new JwtPayload(Issuer,
                                                Audience,
                                                Claims,
                                                DateTime.UtcNow,
                                                DateTime.UtcNow.Add(ValidityDuration),
                                                DateTime.UtcNow);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(Header, Payload);

            return await Task.FromResult(SecurityToken);
        }

    }
}
