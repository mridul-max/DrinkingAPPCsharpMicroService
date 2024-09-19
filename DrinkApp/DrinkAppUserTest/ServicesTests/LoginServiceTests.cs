using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model;
using Users.Model.CustomException;
using Users.Repositories;
using Users.Security;
using Users.Services;

namespace DrinkAppUserTest.ServicesTests
{
    [TestFixture]
    public class LoginServiceTests
    {
        private Fixture _fixture;
        private MockRepository _mockRepository;
        private IPatientRepository _patientRepository;
        private ICareGiverRepository _careGiverRepository;
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<ICareGiverRepository> _careGiverRepositoryMock;
        private Mock<IPatientService> _userServiceMock;
        private IPatientService _userService;
        private Mock<ILoginService> _loginServiceMock;
        private ILoginService _loginService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _careGiverRepositoryMock = _mockRepository.Create<ICareGiverRepository>();
            _careGiverRepository = _careGiverRepositoryMock.Object;
            _patientRepositoryMock = _mockRepository.Create<IPatientRepository>();
            _loginServiceMock = _mockRepository.Create<ILoginService>();
            _loginService = _loginServiceMock.Object;
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
        public async Task Get_Login_UserRole_Should_Return_List_Of_UserRoles()
        {
            //Arrange
            var loginRequest = _fixture.Build<LoginRequest>().Create(); ;
            List<UserRole> userRoles = _fixture.Build<List<UserRole>>().Create();
            _loginServiceMock.Setup(x => x.GetLoginRole(loginRequest)).ReturnsAsync(userRoles);

            // Act
            var result = await _loginService.GetLoginRole(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(userRoles);
        }
        [Test]
        public async Task Get_Login_UserRole_Should_Throw_Exception()
        {
            // Arrange
            _loginServiceMock.Setup(x => x.GetLoginRole(null)).ThrowsAsync(new NotFoundException($"Your phone number or password is incorrecct"));

            // Act
            Func<Task> f = async () =>
            {
                await _loginService.GetLoginRole(null);
            };

            //Assert
            var exception = await f.Should()
                .ThrowAsync<NotFoundException>($"Your phone number or password is incorrecct");
        }
    }
}
