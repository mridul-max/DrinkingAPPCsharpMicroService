using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model.CustomException;
using Users.Model.DTO;
using Users.Repositories;
using Users.Services;

namespace DrinkAppUserTest.ServicesTests
{
    [TestFixture]
    public class EmailServiceTests
    {
        private Fixture _fixture;
        private MockRepository _mockRepository;
        private IPatientRepository _patientRepository;
        private ICareGiverRepository _careGiverRepository;
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<ICareGiverRepository> _careGiverRepositoryMock;
        private Mock<IPatientService> _userServiceMock;
        private IPatientService _userService;
        private Mock<IEmailService> _emailServiceMock;
        private IEmailService _emailService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _careGiverRepositoryMock = _mockRepository.Create<ICareGiverRepository>();
            _careGiverRepository = _careGiverRepositoryMock.Object;
            _patientRepositoryMock = _mockRepository.Create<IPatientRepository>();
            _emailServiceMock = _mockRepository.Create<IEmailService>();
            _emailService = _emailServiceMock.Object;
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
        public async Task SendEmail_Should_Send_Email_To_User()
        {
            // Arrange
            var email = _fixture.Build<SendEmailDTO>().Create();
            var phoneNumber = _fixture.Build<string>().Create();
            _emailServiceMock.Setup(x => x.SendEmailToResetPassword(email, phoneNumber));

            // Act
            await _emailService.SendEmailToResetPassword(email, phoneNumber);
        }
        [Test]
        public async Task SendEmail_Should_Throw_Exception_Send_Email_To_User()
        {
            // Arrange
            var email = _fixture.Build<SendEmailDTO>().Create();
            string phoneNumber = null;
            _emailServiceMock.Setup(x => x.SendEmailToResetPassword(email, phoneNumber))
                .ThrowsAsync(new NotFoundException($"this is not valid phone number {phoneNumber}"));

            // Act
            Func<Task> f = async () =>
            {
                await _emailService.SendEmailToResetPassword(email, phoneNumber);
            };

            //Assert
            var exception = await f.Should()
                .ThrowAsync<NotFoundException>($"this is not valid phone number {phoneNumber}");

        }
    }
}