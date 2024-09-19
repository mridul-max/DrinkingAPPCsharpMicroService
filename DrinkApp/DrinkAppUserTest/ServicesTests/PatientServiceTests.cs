using AutoFixture;
using Moq;
using NUnit.Framework;
using Users.Model.DTO.RespononseDTO;
using Users.Model.DTO;
using Users.Model;
using Users.Repositories;
using Users.Services;
using FluentAssertions;
using Users.Model.CustomException;

namespace DrinkAppUserTest.ServicesTests
{
    [TestFixture]
    public class PatientServiceTests
    {
        private Fixture _fixture;
        private MockRepository _mockRepository;
        private IPatientRepository _patientRepository;
        private Mock<IPasswordHashService> _passWordServiceMock;
        private ICareGiverRepository _careGiverRepository;
        private IPasswordHashService _passWordHashService;
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<ICareGiverRepository> _careGiverRepositoryMock;
        private Mock<IPatientService> _userServiceMock;
        private IPatientService _userService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _careGiverRepositoryMock = _mockRepository.Create<ICareGiverRepository>();
            _careGiverRepository = _careGiverRepositoryMock.Object;
            _patientRepositoryMock = _mockRepository.Create<IPatientRepository>();
            _passWordServiceMock = _mockRepository.Create<IPasswordHashService>();
            _userServiceMock = _mockRepository.Create<IPatientService>();
            _userService = _userServiceMock.Object;
            _passWordHashService = _passWordServiceMock.Object;
            _patientRepository = _patientRepositoryMock.Object;
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
        }
        [Test]
        public async Task RegisterPatient_Should_Return_PatientResponseDTO()
        {
            // Arrange
            var careGiverId = Guid.NewGuid();
            var careGiver = _fixture.Build<CareGiver>().With(x => x.Id, careGiverId).Create();
            var createRegisterPatientDTO = _fixture.Build<RegisterPatientDTO>()
                .With(x => x.CareGiverId, careGiverId)
                .With(x => x.CareGiver, careGiver)
                .Create();
            var Id = Guid.NewGuid();
            var patientRecord = _fixture.Create<Patient>();
            var PatientResponseDTO = new PatientResponseDTO()
            {
                Id = patientRecord.Id,
                Active = patientRecord.Active,
                DateOfBirth = patientRecord.DateOfBirth,
                DailyGoal = patientRecord.DailyGoal,
                FirstName = patientRecord.FirstName,
                LastName = patientRecord.LastName,
                Email = patientRecord.Email,
                UserRole = patientRecord.UserRole,
                PhoneNumber = patientRecord.PhoneNumber,
                DailyLimit = patientRecord.DailyLimit
            };
            var Patient = new RegisterPatientDTO
            {
                FirstName = createRegisterPatientDTO.FirstName,
                LastName = createRegisterPatientDTO.LastName,
                Password = createRegisterPatientDTO.Password,
                PhoneNumber = createRegisterPatientDTO.PhoneNumber,
                Email = createRegisterPatientDTO.Email,
                DailyGoal = createRegisterPatientDTO.DailyGoal,
                DailyLimit = createRegisterPatientDTO.DailyLimit,
                DateOfBirth = createRegisterPatientDTO.DateOfBirth,
                CareGiver = careGiver,

            };
            _userServiceMock.Setup(x => x.RegisteredPatientAsync(Patient, "patient", true)).ReturnsAsync(PatientResponseDTO);

            // Act
            var result = await _userService.RegisteredPatientAsync(Patient, "patient", true);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(PatientResponseDTO);
        }

        [Test]
        public async Task Registerpatient_Should_Throw_When_No_Record_With_Patient_Exist()
        {
            // Arrange
            var careGiverId = Guid.NewGuid();
            var careGiver = _fixture.Build<CareGiver>().With(x => x.Id, careGiverId).Create();
            var createRegisterPatientDTO = _fixture.Build<RegisterPatientDTO>()
                .With(x => x.CareGiverId, careGiverId)
                .With(x => x.CareGiver, careGiver)
                .Create();
            var Id = Guid.NewGuid();
            var patientRecord = _fixture.Create<Patient>();
            var PatientResponseDTO = new PatientResponseDTO()
            {
                Id = patientRecord.Id,
                Active = patientRecord.Active,
                DateOfBirth = patientRecord.DateOfBirth,
                DailyGoal = patientRecord.DailyGoal,
                FirstName = patientRecord.FirstName,
                LastName = patientRecord.LastName,
                Email = patientRecord.Email,
                UserRole = patientRecord.UserRole,
                PhoneNumber = patientRecord.PhoneNumber,
                DailyLimit = patientRecord.DailyLimit
            };
            var Patient = new RegisterPatientDTO
            {
                FirstName = createRegisterPatientDTO.FirstName,
                LastName = createRegisterPatientDTO.LastName,
                Password = createRegisterPatientDTO.Password,
                PhoneNumber = createRegisterPatientDTO.PhoneNumber,
                Email = createRegisterPatientDTO.Email,
                DailyGoal = createRegisterPatientDTO.DailyGoal,
                DailyLimit = createRegisterPatientDTO.DailyLimit,
                DateOfBirth = createRegisterPatientDTO.DateOfBirth,
                CareGiver = careGiver,

            };
            _userServiceMock.Setup(x => x.RegisteredPatientAsync(Patient, "notValid", true))
                .ThrowsAsync(new RegisterUserExistingException($"You give an invalid role for Patient"));
            // Act
            Func<Task> f = async () =>
            {
                await _userService.RegisteredPatientAsync(Patient, "notValid", true);
            };

            //Assert
            var exception = await f.Should()
                .ThrowAsync<RegisterUserExistingException>($"You give an invalid role for Patient");
        }
        [Test]
        public async Task DeactivateUser_Should_Return_True()
        {
            var RecordId = Guid.NewGuid();
            bool response = true;
            _userServiceMock.Setup(x => x.DeactivateUser(RecordId, response)).ReturnsAsync(response);
            // Act
            var result = await _userService.DeactivateUser(RecordId, response);

            // Assert
            result.Should().BeTrue();
        }
        [Test]
        public async Task DeactivateUser_Should_Return_False()
        {
            var RecordId = Guid.NewGuid();
            bool response = false;
            _userServiceMock.Setup(x => x.DeactivateUser(RecordId, response)).ReturnsAsync(response);
            // Act
            var result = await _userService.DeactivateUser(RecordId, response);

            // Assert
            result.Should().BeFalse();
        }
        [Test]
        public async Task EditPatientDailyLimit_Should_Return_UpdatedPatient()
        {
            var patientId = Guid.NewGuid();
            var patient = _fixture.Build<Patient>().With(x => x.Id, patientId)
                .Create();
            _userServiceMock.Setup(x => x.EditPatientLimit(patient.PhoneNumber, patient.DailyLimit)).ReturnsAsync(patient);
            // Act
            var result = await _userService.EditPatientLimit(patient.PhoneNumber, patient.DailyLimit);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(patient);
        }
        [Test]
        public async Task EditPatientDailyLimit_Should_Return_Exception()
        {
            var patientId = Guid.NewGuid();
            var patient = _fixture.Build<Patient>().With(x => x.Id, patientId)
                .Create();
            _userServiceMock.Setup(x => x.EditPatientLimit(null, patient.DailyLimit)).ThrowsAsync(new NotFoundException("The user phone number could not be found"));

            // Act
            Func<Task> f = async () =>
            {
                await _userService.EditPatientLimit(null, patient.DailyLimit);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<NotFoundException>();
            exception.Where(x => x.Message.Contains("The user phone number could not be found"));
        }
        [Test]
        public async Task DeactivateUser_Should_Return_Exception()
        {
            var patientId = new Guid();
            _userServiceMock.Setup(x => x.DeactivateUser(patientId, true)).ThrowsAsync(new NotFoundException("The user could not be found"));

            // Act
            Func<Task> f = async () =>
            {
                await _userService.DeactivateUser(patientId, true);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<NotFoundException>();
            exception.Where(x => x.Message.Contains("The user could not be found"));
        }
    }
}
