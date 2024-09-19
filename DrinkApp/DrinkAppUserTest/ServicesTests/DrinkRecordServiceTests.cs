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
using Users.Model.DTO;
using Users.Model.DTO.RespononseDTO;
using Users.Repositories;
using Users.Services;

namespace DrinkAppUserTest.ServicesTests
{
    [TestFixture]
    public class DrinkRecordServiceTests
    {
        
        private Fixture _fixture;
        private MockRepository _mockRepository;
        private IDrinkRecordRepository _DrinkRecordRepository;
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<IDrinkRecordRepository> _DrinkRecordRepositoryMock;
        private Mock<IDrinkRecordService> _DrinkRecordServiceMock;
        private IDrinkRecordService _DrinkRecordService;
        private IPatientRepository _patientRepository;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _patientRepositoryMock = _mockRepository.Create<IPatientRepository>();
            _DrinkRecordRepositoryMock = _mockRepository.Create<IDrinkRecordRepository>();
            _DrinkRecordServiceMock = _mockRepository.Create<IDrinkRecordService>();
            _DrinkRecordService = _DrinkRecordServiceMock.Object;
            _patientRepository = _patientRepositoryMock.Object;
            _DrinkRecordRepository = _DrinkRecordRepositoryMock.Object;
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
        }
        [Test]
        public async Task AddDrinkRecord_Should_Return_DrinkRecordResponseDTO()
        {
            var RecordId = Guid.NewGuid();
            var DrinksRecord = _fixture.Build<DrinkRecord>().With(x => x.PatientId, RecordId)
                .Create();
            AddDrinkRecordDTO dto = new AddDrinkRecordDTO
            {
                DateTime = DateTime.Now,
                Mililiters = DrinksRecord.Mililiters,
                patient = DrinksRecord.Patient,
                PatientPhonenumber = DrinksRecord.Patient.PhoneNumber
            };
            DrinkRecordResponseDTO responseDTO = new DrinkRecordResponseDTO
            {
                DateTime = DateTime.Now,
                Mililiters = dto.Mililiters
            };
            _DrinkRecordServiceMock.Setup(x => x.AddDrinkRecordAsync(dto)).ReturnsAsync(responseDTO);
            // Act
            var result = await _DrinkRecordService.AddDrinkRecordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(responseDTO);

        }
        [Test]
        public async Task UpdateDrinkRecord_Should_Return_UpdatedDrinkRecordResponseDTO()
        {
            var RecordId = Guid.NewGuid();
            var DrinkRecord = _fixture.Build<DrinkRecord>().With(x => x.PatientId, RecordId)
                .Create();
            DrinkRecordResponseDTO responseDTO = new DrinkRecordResponseDTO
            {
                DateTime = DateTime.Now,
                Mililiters = DrinkRecord.Mililiters
            };
            _DrinkRecordServiceMock.Setup(x => x.EditDrinkRecord(responseDTO.Mililiters, RecordId)).ReturnsAsync(responseDTO);
            // Act
            var result = await _DrinkRecordService.EditDrinkRecord(responseDTO.Mililiters, RecordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(responseDTO);

        }
        [Test]
        public async Task DailyGoalCheck_Should_Return_DailyGoalResponseDTO()
        {
            var RecordId = Guid.NewGuid();
            string msg = "Congratulations you have reached your daily goal";
            DailyGoalResponseDTO responseDTO = new DailyGoalResponseDTO
            {
                message = msg
            };
            _DrinkRecordServiceMock.Setup(x => x.DailyGoalCheck(RecordId)).ReturnsAsync(responseDTO);
            // Act
            var result = await _DrinkRecordService.DailyGoalCheck(RecordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(responseDTO);

        }
        [Test]
        public async Task GetAllDrinkRecordsforoneday_Should_Return_List_Of_DrinkRecordResponseDTO()
        {
            var RecordId = Guid.NewGuid();
            List<DrinkRecordResponseDTO> responseDTOs = _fixture.Build<List<DrinkRecordResponseDTO>>().Create();
            _DrinkRecordServiceMock.Setup(x => x.GetAllDrinkRecordsforoneday(RecordId, null)).ReturnsAsync(responseDTOs);
            // Act
            var result = await _DrinkRecordService.GetAllDrinkRecordsforoneday(RecordId, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(responseDTOs);

        }
        [Test]
        public async Task AddDrinkRecord_Should_Throw_Exception()
        {
            AddDrinkRecordDTO dto = null;
            _DrinkRecordServiceMock.Setup(x => x.AddDrinkRecordAsync(dto)).ThrowsAsync(new CouldNotRecordException("Could not Add to db"));

            // Act
            Func<Task> f = async () =>
            {
                await _DrinkRecordService.AddDrinkRecordAsync(dto);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<CouldNotRecordException>();
            exception.Where(x => x.Message.Contains("Could not Add to db"));
        }
        [Test]
        public async Task EditDrinkRecord_Should_throw_Exception()
        {
            var patientId = new Guid();
            _DrinkRecordServiceMock.Setup(x => x.EditDrinkRecord(500, patientId)).ThrowsAsync(new UpdateFailedException("Record could not be updated"));

            // Act
            Func<Task> f = async () =>
            {
                await _DrinkRecordService.EditDrinkRecord(500, patientId);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<UpdateFailedException>();
            exception.Where(x => x.Message.Contains("Record could not be updated"));
        }
        [Test]
        public async Task GetAllDrinkRecordsforoneday_Should_throw_Exception()
        {
            var patientId = new Guid();
            _DrinkRecordServiceMock.Setup(x => x.GetAllDrinkRecordsforoneday(patientId, null)).ThrowsAsync(new EntitiesNotFoundException("Could not find any records for the day"));

            // Act
            Func<Task> f = async () =>
            {
                await _DrinkRecordService.GetAllDrinkRecordsforoneday(patientId, null);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<EntitiesNotFoundException>();
            exception.Where(x => x.Message.Contains("Could not find any records for the day"));
        }
        [Test]
        public async Task DailyGoalCheck_Should_throw_Exception()
        {
            var patientId = new Guid();
            _DrinkRecordServiceMock.Setup(x => x.DailyGoalCheck(patientId)).ThrowsAsync(new EntitiesNotFoundException("Could not find any records for the day"));

            // Act
            Func<Task> f = async () =>
            {
                await _DrinkRecordService.DailyGoalCheck(patientId);
            };

            // Assert
            var exception = await f.Should().ThrowAsync<EntitiesNotFoundException>();
            exception.Where(x => x.Message.Contains("Could not find any records for the day"));
        }
    }
}
