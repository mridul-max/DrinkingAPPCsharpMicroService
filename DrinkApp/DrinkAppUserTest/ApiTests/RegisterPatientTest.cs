/*using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NUnit.Framework;
using System.Net;
using Users;
using Users.AppCrudRepositories;
using Users.Model;
using Users.Model.DTO;

namespace DrinkAppUserTest.ApiTests
{
    public class RegisterPatientTest : IntegrationTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }
        [Test]
        public async Task RegisterPatient_Returns_Response_Created()
        {
            //Arrange
            UserRole role = new UserRole
            {
                Role = Role.PATIENT
            };
            CareGiver careGiver = new CareGiver();
            Guid careGiverId = Guid.NewGuid();
            var registerPatientDTO = _fixture.Build<RegisterPatientDTO>()
                .With(x => x.Email, "mahedi@gmail.com")
                .With(x => x.LastName, "mahedi")
                .With(x => x.FirstName, "Mridul")
                .With(x => x.PhoneNumber, "+31645826735")
                .With(x => x.Password, "Mahedi0134!")
                .With(x => x.DailyGoal, 12)
                .With(x => x.DateOfBirth, new DateTime(1950, 05, 11))
                .Create();

            //Act
            var response = await _httpClient.PostAsJsonAsync("register/patient", registerPatientDTO);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
*/