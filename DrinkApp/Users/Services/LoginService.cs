using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model;
using Users.Model.CustomException;
using Users.Repositories;
using Users.Security;

namespace Users.Services
{
    public class LoginService : ILoginService
    {
        private ILogger _logger { get; }
        private IPatientRepository _patientGenericRepository { get; }
        private ICareGiverRepository _careGiverGenericRepository { get; }
        IPasswordHashService _passwordHashService { get; }
        public LoginService(ILogger<LoginService> logger, IPasswordHashService PasswordHashService
            , IPatientRepository patientRepository, ICareGiverRepository careGiverGenericRepository)
        {
            _logger = logger;
            _passwordHashService = PasswordHashService;
            _patientGenericRepository = patientRepository;
            _careGiverGenericRepository = careGiverGenericRepository;
        }

        public async Task<List<UserRole>> GetLoginRole(LoginRequest loginRequest)
        {
            List<UserRole> Roles = new List<UserRole>();
            var patient = await _patientGenericRepository.GetByPhoneNumber(loginRequest.PhoneNumber);
            var careGiver = await _careGiverGenericRepository.GetByPhoneNumber(loginRequest.PhoneNumber);
            if (patient == null && careGiver == null)
            {
                throw new NotFoundException("Your phone number or password is incorrecct");
            }
            if (patient != null && _passwordHashService.ValidatePassword(patient.Password))
            {
                Roles.Add(patient.UserRole);
            }
            else if (careGiver != null && _passwordHashService.ValidatePassword(careGiver.Password))
            {
                foreach (UserRole u in careGiver.UserRoles)
                {
                    Roles.Add(u);
                }
                return Roles;
            }
            return Roles;
        }
    }
}
