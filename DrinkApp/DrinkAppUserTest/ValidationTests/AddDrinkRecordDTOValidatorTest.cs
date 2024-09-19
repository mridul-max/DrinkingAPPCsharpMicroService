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
    internal class AddDrinkRecordDTOValidatorTest
    {
        private AddDrinkRecordDTOValidator _validator;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _validator = new AddDrinkRecordDTOValidator();
        }

        [TestCase("+31645826735", "100")]
        public void Add_DrinkRecord_DTO_Validator_Shoud_Accept_valid_Properties(string phoneNumber,
            int ml)
        {
            List<DrinkRecord> drinkRecords = new List<DrinkRecord>();
            Patient patient = new Patient();
            var saveDrink = _fixture.Build<AddDrinkRecordDTO>()
                .With(x => x.Mililiters, ml)
                .With(x => x.PatientPhonenumber, phoneNumber)
                .With(x => x.patient, patient)
                .Create();
            var result = _validator.TestValidate(saveDrink);
            result.IsValid.Should().BeTrue();
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("+3142")]
        public void Should_Have_Validation_Errors_For_Invalid_PhoneNumber(string PhoneNumber)
        {
            List<DrinkRecord> drinkRecords = new List<DrinkRecord>();
            Patient patient = new Patient();
            var savedDrinks = _fixture.Build<AddDrinkRecordDTO>().With(x => x.PatientPhonenumber, PhoneNumber).With(x=>x.patient,patient)
                //.With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(savedDrinks);
            result.ShouldHaveValidationErrorFor(x => x.PatientPhonenumber);
        }
        [TestCase(-2)]
        [TestCase(0)]
        public void Should_Have_Validation_Errors_For_Invalid_Mililitres(int ml)
        {
            List<DrinkRecord> drinkRecords = new List<DrinkRecord>();
            Patient patient = new Patient();
            var savedDrinks = _fixture.Build<AddDrinkRecordDTO>().With(x => x.Mililiters, ml).With(x => x.patient, patient)
                //.With(x => x.Patients, patients)
                .Create();
            var result = _validator.TestValidate(savedDrinks);
            result.ShouldHaveValidationErrorFor(x => x.Mililiters);
        }
    }
}
