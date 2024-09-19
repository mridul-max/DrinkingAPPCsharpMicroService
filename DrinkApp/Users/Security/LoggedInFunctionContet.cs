using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Users.Security
{
    public static class LoggedInFunctionContet
    {
        public static ClaimsPrincipal GetUser(this FunctionContext context)
        {
            if (context.Items.TryGetValue("User", out object user))
            {
                return (ClaimsPrincipal)user;
            }
            return null;

        }
    }
}
