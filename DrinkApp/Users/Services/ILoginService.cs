using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model;
using Users.Security;

namespace Users.Services
{
    public interface ILoginService
    {
        Task<List<UserRole>> GetLoginRole(LoginRequest loginRequest);
    }
}
