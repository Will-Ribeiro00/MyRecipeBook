using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validator.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            //Arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.ShouldBeTrue();
        }
        [Fact]
        public void ErrorNameEmpty()
        {
            //Arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.NAME_EMPTY));
        }
        [Fact]
        public void ErrorEmailEmpty()
        {
            //Arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = string.Empty;

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_EMPTY));
        }
        [Fact]
        public void ErrorEmailInvalid()
        {
            //Arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "email_invalido.com";

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_INVALID));
        }
        [Fact]
        public void ErrorPasswordEmpty()
        {
            //Arrange
            var validator = new RegisterUserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Password = string.Empty;

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.PASSWORD_INVALID));
        }
    }
}
