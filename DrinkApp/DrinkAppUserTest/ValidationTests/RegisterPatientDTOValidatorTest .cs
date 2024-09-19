using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;
using Users.Model;
using Users.Model.DTO;
using Users.Validation;

namespace Users.ValidationTests
{
    [TestFixture]
    internal class RegisterPatientDTOValidatorTest
    {
        private RegisterPatientDTOValidator _validator;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _validator = new RegisterPatientDTOValidator();
        }

        //email validating
        [TestCase("mah")]
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Email(string email)
        {
            CareGiver careGiver = new CareGiver();
            var saveEmail = _fixture.Build<RegisterPatientDTO>().With(x => x.Email, email)
                .With(x => x.CareGiver, careGiver)
                .Create();
            var result = _validator.TestValidate(saveEmail);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [TestCase("mahedimridul57@gmail.com", "mah", 
        "di", "+31645826735", "Tanzel0179!", "False", 2,"1980-09-09")]
        public void Register_Patient_DTO_Validator_Shoud_Accept_valid_Properties(string email, 
            string firstName, string lastname, string phoneNumber,
            string password, bool active, int dailyGoal, DateTime dateOfBirth)
        {
            UserRole role = new UserRole{
             Role = Role.PATIENT 
            };
            CareGiver careGiver = new CareGiver();
            Guid careGiverId = Guid.NewGuid();
            var saveEmail = _fixture.Build<RegisterPatientDTO>()
                .With(x => x.Email, email)
                .With(x => x.LastName, lastname)
                .With(x => x.FirstName, firstName)
                .With(x => x.PhoneNumber, phoneNumber)
                .With(x => x.Password, password)
                .With(x => x.DailyGoal, dailyGoal)
                .With(x => x.DateOfBirth, dateOfBirth)
                .With(x => x.CareGiver, careGiver)
                .With(x => x.CareGiverId, careGiverId)
                .Create();
            var result = _validator.TestValidate(saveEmail);
            result.IsValid.Should().BeTrue();
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("+3142")]
        public void Should_Have_Validation_Errors_For_Invalid_PhoneNumber(string PhoneNumber)
        {
            CareGiver careGiver = new CareGiver(); 
            var savedPhoneNumber = _fixture.Build<RegisterPatientDTO>().With(x => x.PhoneNumber, PhoneNumber)
                  .With(x => x.CareGiver, careGiver)
                  .Create();
            var result = _validator.TestValidate(savedPhoneNumber);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("hjdshw1")]
        [TestCase("dshfdsywy37432764263277dsdshdhy")]
        public void Should_Have_Validation_Errors_For_Invalid_NewPassword(string NewPassword)
        {
            CareGiver careGiver = new CareGiver();
            var savedPassword = _fixture.Build<RegisterPatientDTO>().With(x => x.Password, NewPassword)
                 .With(x => x.CareGiver, careGiver)
                 .Create();
            var result = _validator.TestValidate(savedPassword);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        //Naming validating
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Firstname(string firstName)
        {
            CareGiver careGiver = new CareGiver();
            var saveFirstname = _fixture.Build<RegisterPatientDTO>().With(x => x.FirstName, firstName)
                  .With(x => x.CareGiver, careGiver)
                  .Create(); ;
            var result = _validator.TestValidate(saveFirstname);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Lastname(string lastname)
        {
            CareGiver careGiver = new CareGiver(); 
            var saveLastname = _fixture.Build<RegisterPatientDTO>().With(x => x.LastName, lastname)
                  .With(x => x.CareGiver, careGiver)
                  .Create();
            var result = _validator.TestValidate(saveLastname);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }
    }
}