using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Model;
using Users.Model.CustomException;
using Users.Model.DTO;
using Users.Repositories;

namespace Users.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        private ILogger _logger { get; }
        private IPatientRepository _patientGenericRepository { get; }
        private ICareGiverRepository _careGiverGenericRepository { get; }
        public PasswordHashService(ILogger<PasswordHashService> logger, IPatientRepository patientRepository, ICareGiverRepository careGiverGenericRepository)
        {
            _logger = logger;
            _patientGenericRepository = patientRepository;
            _careGiverGenericRepository = careGiverGenericRepository;
        }
        public string HashPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }
        public bool ValidatePassword(string providedPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, storedHashedPassword);
        }
        public async Task PasswordResetByTokenCode(ForgetPasswordDTO ForgetPasswordDTO, string PhoneNumber)
        {
            var patient = await _patientGenericRepository.GetByPhoneNumber(PhoneNumber);
            var careGiver = await _careGiverGenericRepository.GetByPhoneNumber(PhoneNumber);
            if (patient == null && careGiver == null)
            {
                throw new NotFoundException("Your phone number is not found");
            }
            if (patient != null)
            {
                TimeSpan span = DateTime.Now.Subtract(patient.TokenCodeGeneratedTime);
                if (patient.GenerateTokenCode.Contains(ForgetPasswordDTO.GenerateTokenCode) && span.Hours <= 1)
                {
                    patient.Password = HashPassword(ForgetPasswordDTO.NewPassword);
                    patient.GenerateTokenCode = null;
                    _patientGenericRepository.Update(patient);
                }
            }
            else if (careGiver != null)
            {
                TimeSpan span = DateTime.Now.Subtract(careGiver.TokenCodeGeneratedTime);
                if (careGiver.GenerateTokenCode.Contains(ForgetPasswordDTO.GenerateTokenCode) && span.Hours <= 1)
                {
                    careGiver.Password = HashPassword(ForgetPasswordDTO.NewPassword);
                    careGiver.GenerateTokenCode = null;
                    _careGiverGenericRepository.Update(careGiver);
                }
            }
        }
    }
}
