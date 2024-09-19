using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Users.Model;
using Users.Model.DTO;
using Users.Validation;

namespace Users.ValidationTests
{
    [TestFixture]
    internal class RegisterCareGiverDTOValidatorTest
    {
        private RegisterCareGiverDTOValidator _validator;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _validator = new RegisterCareGiverDTOValidator();
        }

        //email validating
        [TestCase("mah")]
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Email(string email)
        {
            List<Patient> patients = new List<Patient>();
            var saveEmail = _fixture.Build<RegisterCareGiverDTO>().With(x => x.Email, email)
                .With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(saveEmail);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [TestCase("mahedimridul57@gmail.com", "mah", 
        "di", "+31645826735", "Tanzel0179!", "False")]
        public void Register_CareGiver_DTO_Validator_Shoud_Accept_valid_Properties(string email, 
            string firstName, string lastname, string phoneNumber,
            string password, bool active)
        {
            List<Patient> patients = new List<Patient>();
            List<UserRole> roles = new List<UserRole>()
            {
                new UserRole
                {
                    Role = Role.CARE_GIVER,
                }
            };
            var saveEmail = _fixture.Build<RegisterCareGiverDTO>()
                .With(x => x.Email, email)
                .With(x => x.LastName, lastname)
                .With(x => x.FirstName, firstName)
                .With(x => x.PhoneNumber, phoneNumber)
                .With(x => x.Password, password)
                .With(x => x.Patients, patients)
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
            List<Patient> patients = new List<Patient>();
            var savedPhoneNumber = _fixture.Build<RegisterCareGiverDTO>().With(x => x.PhoneNumber, PhoneNumber)
                .With(x => x.Patients, patients)
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
            List<Patient> patients = new List<Patient>();
            var savedPassword = _fixture.Build<RegisterCareGiverDTO>().With(x => x.Password, NewPassword)
                .With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(savedPassword);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        //Naming validating
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Firstname(string firstName)
        {
            List<Patient> patients = new List<Patient>();
            var saveFirstname = _fixture.Build<RegisterCareGiverDTO>().With(x => x.FirstName, firstName)
                .With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(saveFirstname);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Lastname(string lastname)
        {
            List<Patient> patients = new List<Patient>();
            var saveLastname = _fixture.Build<RegisterCareGiverDTO>().With(x => x.LastName, lastname)
                .With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(saveLastname);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }
    }
}