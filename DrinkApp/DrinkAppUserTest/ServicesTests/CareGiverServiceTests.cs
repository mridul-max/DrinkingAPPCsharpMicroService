using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Users.Model.DTO.RespononseDTO;
using Users.Model.CustomException;
using Users.Model.DTO;
using Users.Model;
using Users.Repositories;
using Users.Services;

namespace DrinkAppUserTest.ServicesTests
{
    [TestFixture]
    public class CareGiverServiceTests
    {
        private Fixture _fixture;
        private MockRepository _mockRepository;
        private IPatientRepository _patientRepository;
        private Mock<IPasswordHashService> _passWordServiceMock;
        private ICareGiverRepository _careGiverRepository;
        private IPasswordHashService _passWordHashService;
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<ICareGiverRepository> _careGiverRepositoryMock;
        private Mock<ICareGiverService> _userServiceMock;
        private ICareGiverService _userService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _careGiverRepositoryMock = _mockRepository.Create<ICareGiverRepository>();
            _careGiverRepository = _careGiverRepositoryMock.Object;
            _patientRepositoryMock = _mockRepository.Create<IPatientRepository>();
            _passWordServiceMock = _mockRepository.Create<IPasswordHashService>();
            _userServiceMock = _mockRepository.Create<ICareGiverService>();
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
        public async Task RegisterCareGiver_Should_Throw_When_No_Record_With_Patient_Exist()
        {
            // Arrange
            var careGiverId = Guid.NewGuid();
            var careGiver = _fixture.Build<CareGiver>().With(x => x.Id, careGiverId)
                .Create();
            var CareGiverResponseDTO = new CareGiverResponseDTO()
            {
                Id = careGiverId,
                Roles = careGiver.UserRoles,
                Email = careGiver.Email,
                FirstName = careGiver.FirstName,
                LastName = careGiver.LastName,
                PhoneNumber = careGiver.PhoneNumber
            };
            var careGiverDTO = new RegisterCareGiverDTO
            {
                FirstName = careGiver.FirstName,
                LastName = careGiver.LastName,
                Password = careGiver.Password,
                PhoneNumber = careGiver.PhoneNumber,
                Email = careGiver.Email,
            };
            _userServiceMock.Setup(x => x.RegisteredCareGiverAsync(careGiverDTO, "patient", true)).ThrowsAsync(new RegisterUserExistingException($"You give an invalid role for careGiver"));

            //Act
            Func<Task> f = async () =>
            {
                await _userService.RegisteredCareGiverAsync(careGiverDTO, "patient", true);
            };

            //Assert
            var exception = await f.Should()
                .ThrowAsync<RegisterUserExistingException>($"You give an invalid role for careGiver");
        }
        [Test]
        public async Task RegisterCareGiver_Should_Return_CareGiverResponseDTO()
        {
            // Arrange
            var careGiverId = Guid.NewGuid();
            var careGiver = _fixture.Build<CareGiver>().With(x => x.Id, careGiverId)
                .Create();
            var CareGiverResponseDTO = new CareGiverResponseDTO()
            {
                Id = careGiverId,
                Roles = careGiver.UserRoles,
                Email = careGiver.Email,
                FirstName = careGiver.FirstName,
                LastName = careGiver.LastName,
                PhoneNumber = careGiver.PhoneNumber
            };
            var careGiverDTO = new RegisterCareGiverDTO
            {
                FirstName = careGiver.FirstName,
                LastName = careGiver.LastName,
                Password = careGiver.Password,
                PhoneNumber = careGiver.PhoneNumber,
                Email = careGiver.Email,
            };
            _userServiceMock.Setup(x => x.RegisteredCareGiverAsync(careGiverDTO, "care_giver", true)).ReturnsAsync(CareGiverResponseDTO);

            // Act
            var result = await _userService.RegisteredCareGiverAsync(careGiverDTO, "care_giver", true);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(CareGiverResponseDTO);
        }
    }
}
