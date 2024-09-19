using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model.DTO.RespononseDTO;
using Users.Model.DTO;
using Users.Model;

namespace Users.Services
{
    public interface IPatientService
    {
        Task<PatientResponseDTO> RegisteredPatientAsync(RegisterPatientDTO registeredUser, string role, bool status);
        Task<Patient> EditPatientLimit(string phoneNo, int newlimit);
        Task<bool> DeactivateUser(Guid id, bool status);
        Task<List<PatientResponseDTO>> GetAllPatients();
        Task<PatientResponseDTO> GetOnePatient(Guid id);
    }
}
