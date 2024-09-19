using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model.DTO;

namespace Users.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(string Password);
        bool ValidatePassword(string Password);
        Task PasswordResetByTokenCode(ForgetPasswordDTO ForgetPasswordDTO, string PhoneNumber);
    }
}
