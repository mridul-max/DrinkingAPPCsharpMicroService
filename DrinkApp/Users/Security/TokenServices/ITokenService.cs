using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Users.Security.TokenServices
{
    public interface ITokenService
    {
        Task<LoginResult> CreateToken(LoginRequest login);
        Task<ClaimsPrincipal> GetByValue(string value);   
    }
}
