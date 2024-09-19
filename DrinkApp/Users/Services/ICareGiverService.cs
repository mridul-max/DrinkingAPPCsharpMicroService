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
    public interface ICareGiverService
    {
        Task<CareGiverResponseDTO> RegisteredCareGiverAsync(RegisterCareGiverDTO dto, string role, bool status);
        Task AssignPatient(Guid careGiverId, Guid patientId);
        Task<CareGiverResponseDTO> GetOneCareGiver(Guid id);
        Task<List<CareGiverResponseDTO>> GetAllCareGivers();
    }
}
