using System;
using System.Threading.Tasks;
using Users.Model;
using Microsoft.Extensions.Logging;
using Users.Model.CustomException;
using Users.Model.DTO.RespononseDTO;
using Users.Model.DTO;
using AutoMapper;
using Users.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Users.Services
{
    public class PatientService : IPatientService   
    {
        private ILogger _logger { get; }
        private IPatientRepository _patientGenericRepository { get; }
        private ICareGiverRepository _careGiverGenericRepository { get; }  
        IPasswordHashService _passwordHashService { get; }
        public PatientService(ILogger<PatientService> logger, IPasswordHashService PasswordHashService,
               IEmailService EmailService, IPatientRepository patientRepository, ICareGiverRepository careGiverGenericRepository)
        {
            _logger = logger;
            _passwordHashService = PasswordHashService;
            _patientGenericRepository = patientRepository;
            _careGiverGenericRepository = careGiverGenericRepository;
        }
        public async Task<PatientResponseDTO> RegisteredPatientAsync(RegisterPatientDTO registeredUser, string role, bool status)
        {
            var patientResult = await _patientGenericRepository.GetByPhoneNumber(registeredUser.PhoneNumber);
            var careGiverResult = await _careGiverGenericRepository.GetByPhoneNumber(registeredUser.PhoneNumber);
            if (patientResult != null || careGiverResult != null)
            {
                throw new RegisterUserExistingException($"This User already exist with phoneNumber {registeredUser.PhoneNumber}");
            }
            if (registeredUser.CareGiverId != Guid.Empty)
            {
                registeredUser.CareGiver = await _careGiverGenericRepository.GetByIdAsync(registeredUser.CareGiverId);
            }
            if ((Role)Enum.Parse(typeof(Role), role.ToUpper()) != Role.PATIENT)
            {
                throw new RegisterUserExistingException($"You give an invalid role for Patient");
            }
            var patient = MapDTOToPatient(registeredUser);
            patient.Id = Guid.NewGuid();
            patient.UserRole = new UserRole { Role = (Role)Enum.Parse(typeof(Role), role.ToUpper()) };
            patient.Active = status;
            patient.Password = _passwordHashService.HashPassword(registeredUser.Password);
            _patientGenericRepository.Insert(patient);
            _patientGenericRepository.Save();
            var response = MapDTOToPatientResponse(patient);
            return response;
        }
        public Patient MapDTOToPatient(RegisterPatientDTO registerUserDTO)
        {
            var map = new MapperConfiguration(mp =>
            {
                mp.CreateMap<RegisterPatientDTO, Patient>();
            });
            IMapper iMap = map.CreateMapper();
            return iMap.Map<RegisterPatientDTO, Patient>(registerUserDTO);
        }
        public PatientResponseDTO MapDTOToPatientResponse(Patient patient)
        {
            var map = new MapperConfiguration(mp =>
            {
                mp.CreateMap<Patient, PatientResponseDTO>();
            });
            IMapper iMap = map.CreateMapper();
            return iMap.Map<Patient, PatientResponseDTO>(patient);
        }
        public async Task<bool> DeactivateUser(Guid id, bool status)
        {
            var patient = await _patientGenericRepository.GetByIdAsync(id);
            var caregiver = await _careGiverGenericRepository.GetByIdAsync(id);

            if (patient != null)
            {
                patient.Active = status;
                _patientGenericRepository.Update(patient);
                return patient.Active;
            }
            else if (caregiver != null)
            {
                caregiver.Active = status;
                _careGiverGenericRepository.Update(caregiver);
                return caregiver.Active;
            }
            throw new NotFoundException("The user could not be found");

        }
        public async Task<Patient> EditPatientLimit(string phoneNo, int newlimit)
        {
            var patient = await _patientGenericRepository.GetByPhoneNumber(phoneNo);
            if (patient != null)
            {
                patient.DailyLimit = newlimit;
                _patientGenericRepository.Update(patient);
                return patient;
            }
            throw new NotFoundException("The user phone number could not be found");
        }
        public async Task<PatientResponseDTO> GetOnePatient(Guid id)
        {
            var patient = await _patientGenericRepository.GetByIdAsync(id);
            var responsePatient = MapDTOToPatientResponse(patient);
            if (responsePatient != null)
            {
                return responsePatient;
            }
            else
                throw new NotFoundException("Patient Could not be found");
            
        }
        public async Task<List<PatientResponseDTO>> GetAllPatients()
        {
            var patients =  _patientGenericRepository.GetAll().ToList();
            var patientlist = new List<PatientResponseDTO>();
            foreach (var patient in patients)
            {
                patientlist.Add(MapDTOToPatientResponse(patient));
            }
            if (patientlist.Count > 0)
            {
                return patientlist;
            }
            else
            {
                throw new NotFoundException("Could not find patients");
            }
        }
    }
}
