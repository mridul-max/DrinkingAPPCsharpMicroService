using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Users.Model.DTO;
using Users.Validation;

namespace Users.ValidationTests
{
    [TestFixture]
    public class SendEmailDTOValidatorTest
    {
        private SendEmailDTOValidator _validator;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _validator = new SendEmailDTOValidator();
        }

        //email validating
        [TestCase("mah")]
        [TestCase("")]
        [TestCase(" ")]
        public void Should_Have_Validation_Errors_For_Invalid_Email(string email)
        {
            var sendEmail = _fixture.Build<SendEmailDTO>().With(x => x.Email, email).Create();
            var result = _validator.TestValidate(sendEmail);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [TestCase("mahedimridul57@gmail.com")]
        public void Send_EMail_DTO_Validator_Shoud_Accept_valid_Email(string email)
        {
            var sendEmail = _fixture.Build<SendEmailDTO>().With(x => x.Email, email).Create();
            var result = _validator.TestValidate(sendEmail);
            result.IsValid.Should().BeTrue();
        }
    }
}























// var userNumber = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);