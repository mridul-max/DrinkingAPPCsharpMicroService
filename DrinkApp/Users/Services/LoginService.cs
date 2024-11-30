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
            List<UserRole> roles = new List<UserRole>();

            // Check if the user is a patient
            var patient = await _patientGenericRepository.GetByPhoneNumber(loginRequest.PhoneNumber);
            if (patient != null)
            {
                if (_passwordHashService.ValidatePassword(loginRequest.Password, patient.Password))
                {
                    roles.Add(patient.UserRole);
                    return roles;
                }
                else
                {
                    throw new NotFoundException("Incorrect phone number or password.");
                }
            }

            // Check if the user is a caregiver
            var caregiver = await _careGiverGenericRepository.GetByPhoneNumber(loginRequest.PhoneNumber);
            if (caregiver != null)
            {
                if (_passwordHashService.ValidatePassword(loginRequest.Password, caregiver.Password))
                {
                    roles.AddRange(caregiver.UserRoles);
                    return roles;
                }
                else
                {
                    throw new NotFoundException("Incorrect phone number or password.");
                }
            }

            // If no user found
            throw new NotFoundException("Incorrect phone number or password.");
        }

    }
}
