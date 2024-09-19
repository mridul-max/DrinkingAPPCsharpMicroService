using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model.CustomException;
using Users.Model.DTO.RespononseDTO;
using Users.Model.DTO;
using Users.Model;
using Microsoft.Extensions.Logging;
using Users.Repositories;

namespace Users.Services
{
    public class CareGiverService : ICareGiverService
    {
        private ILogger _logger { get; }
        private IPatientRepository _patientGenericRepository { get; }
        private ICareGiverRepository _careGiverGenericRepository { get; }
        IPasswordHashService _passwordHashService { get; }
        public CareGiverService (ILogger<CareGiverService> logger, IPasswordHashService PasswordHashService,
                IPatientRepository patientRepository, ICareGiverRepository careGiverGenericRepository)
        {
            _logger = logger;
            _passwordHashService = PasswordHashService;
            _patientGenericRepository = patientRepository;
            _careGiverGenericRepository = careGiverGenericRepository;
        }
        public async Task<CareGiverResponseDTO> RegisteredCareGiverAsync(RegisterCareGiverDTO dto, string role, bool status)
        {
            var patientResult = await _patientGenericRepository.GetByPhoneNumber(dto.PhoneNumber);
            var careGiverResult = await _careGiverGenericRepository.GetByPhoneNumber(dto.PhoneNumber);
            if (patientResult != null || careGiverResult != null)
            {
                throw new RegisterUserExistingException($"This User already exist with phoneNumber {dto.PhoneNumber}");
            }
            if ((Role)Enum.Parse(typeof(Role), role.ToUpper()) == Role.PATIENT)
            {
                throw new RegisterUserExistingException($"You give an invalid role for careGiver");
            }
            var careGiver = MapCareGiverDtoToCareGiver(dto);
            careGiver.Id = Guid.NewGuid();
            if ((Role)Enum.Parse(typeof(Role), role.ToUpper()) == Role.CARE_GIVER)
            {
                careGiver.UserRoles = new List<UserRole>
                    {
                        new UserRole { Role = (Role)Enum.Parse(typeof(Role), role.ToUpper()) },
                        new UserRole { Role = Role.ADMIN }
                    };
            }
            else
            {
                careGiver.UserRoles = new List<UserRole>
                    {
                        new UserRole { Role = (Role)Enum.Parse(typeof(Role), role.ToUpper()) },
                    };
            }
            careGiver.Password = _passwordHashService.HashPassword(careGiver.Password);
            careGiver.Active = status;
            _careGiverGenericRepository.Insert(careGiver);
            _careGiverGenericRepository.Save();
            var response = MapDTOToCareGiverResponse(careGiver);
            return response;
        }

        public CareGiver MapCareGiverDtoToCareGiver(RegisterCareGiverDTO dto)
        {
            var map = new MapperConfiguration(mp =>
            {
                mp.CreateMap<RegisterCareGiverDTO, CareGiver>();
            });
            IMapper iMap = map.CreateMapper();
            return iMap.Map<RegisterCareGiverDTO, CareGiver>(dto);
        }
        private CareGiverResponseDTO MapDTOToCareGiverResponse(CareGiver careGiver)
        {
            var map = new MapperConfiguration(mp =>
            {
                mp.CreateMap<CareGiver, CareGiverResponseDTO>();
            });
            IMapper iMap = map.CreateMapper();
            return iMap.Map<CareGiver, CareGiverResponseDTO>(careGiver);
        }
        public async Task AssignPatient(Guid careGiverId, Guid patientId)
        {
           var patient = await _patientGenericRepository.GetByIdAsync(patientId);
           var caregiver = await _careGiverGenericRepository.GetByIdAsync(careGiverId);
            if (caregiver != null)
            {
                patient.CareGiver = caregiver;
                _patientGenericRepository.Update(patient);
            }
            else
                throw new NotFoundException("The id is incorrect");
        }
        public async Task<CareGiverResponseDTO> GetOneCareGiver(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new NotFoundException("The id Can not be Empty");
            }
            var caregiver = await _careGiverGenericRepository.GetByIdAsync(id);            
            return MapDTOToCareGiverResponse(caregiver);
        }
        public async Task<List<CareGiverResponseDTO>> GetAllCareGivers()
        {
            var careGivers = _careGiverGenericRepository.GetAll().ToList();
            var careGiversList = new List<CareGiverResponseDTO>();  
            foreach(var cg in careGivers) 
            { 
                careGiversList.Add(MapDTOToCareGiverResponse(cg));
            }
            if (careGiversList.Count > 0)
            {
                return careGiversList;
            }
            else
            {
                throw new NotFoundException("Could not find caregivers");
            }
            

        }
    }
}
