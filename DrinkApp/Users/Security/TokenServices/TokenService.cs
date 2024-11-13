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
        private ILoginService _loginService { get; }

        public TokenService(ILogger<TokenService> log, ILoginService loginService)
        {
            Logger = log;
            _loginService = loginService;

            // Fetch values directly from environment variables
            Issuer = Environment.GetEnvironmentVariable("Issuer") ?? throw new Exception("Issuer not configured");
            Audience = Environment.GetEnvironmentVariable("Audience") ?? throw new Exception("Audience not configured");
            ValidityDuration = TimeSpan.FromDays(1);
            string key = Environment.GetEnvironmentVariable("Key") ?? throw new Exception("Key not configured");

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, securityKey);
        }

        public async Task<LoginResult> CreateToken(LoginRequest login)
        {
            List<UserRole> userRoles = await _loginService.GetLoginRole(login);
            var claims = new List<Claim>();

            // Add roles as multiple claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.ToString()));
            }
            claims.Add(new Claim(ClaimTypes.NameIdentifier, login.PhoneNumber));

            JwtSecurityToken token = await CreateToken(claims.ToArray());
            return new LoginResult(token);
        }

        public async Task<ClaimsPrincipal> GetByValue(string value)
        {
            if (value == null)
            {
                throw new Exception("No Token supplied");
            }
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken validatedToken;
                ClaimsPrincipal principal = handler.ValidateToken(value, ValidationParameters, out validatedToken);
                return await Task.FromResult(principal);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<JwtSecurityToken> CreateToken(Claim[] claims)
        {
            JwtHeader header = new JwtHeader(Credentials);
            JwtPayload payload = new JwtPayload(Issuer, Audience, claims, DateTime.UtcNow, DateTime.UtcNow.Add(ValidityDuration), DateTime.UtcNow);
            JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);
            return await Task.FromResult(securityToken);
        }
    }
}
