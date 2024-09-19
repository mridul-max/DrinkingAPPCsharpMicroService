using Microsoft.Azure.Functions.Worker;
using System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Model;
using Users.Model.DTO;
using Users.Model.DTO.RespononseDTO;
using Users.Security;

namespace Users.Services
{
    public interface IUserService
    {
        Task<PatientResponseDTO> RegisteredPatientAsync(RegisterPatientDTO registeredUser, string role, bool status);
        Task<CareGiverResponseDTO> RegisteredCareGiverAsync(RegisterCareGiverDTO registeredUser, string role, bool status);
        Task<List<UserRole>> GetLoginRole(LoginRequest loginRequest);
        Task SendEmailToResetPassword(SendEmailDTO sendEmailDTO, string phoneNumber);
        Task PasswordResetByTokenCode(ForgetPasswordDTO ForgetPasswordDTO, string PhoneNumber);
        Task<List<PatientDTO>> GetAllPatients();
        Task<List<CareGiverDTO>> GetAllCareGivers();
        Task AssignPatient(Guid cgId, Guid patientId);
        Task<Patient> EditPatientLimit(string phoneNo, int newlimit);
        Task<bool> DeactivateUser(Guid id, bool status);
    }
}
