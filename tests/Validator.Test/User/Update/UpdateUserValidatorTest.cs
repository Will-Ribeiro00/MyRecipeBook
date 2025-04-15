using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validator.Test.User.Update
{
    public class UpdateUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }
        
        [Fact]
        public void ErrorNameEmpty()
        {
            // Arrange
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(message => message.ErrorMessage.Equals(ResourceMessagesExceptions.NAME_EMPTY));
        }

        [Fact]
        public void ErrorEmailEmpty()
        {
            // Arrange
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = string.Empty;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(message => message.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_EMPTY));
        }

        [Fact]
        public void ErrorEmailInvalid()
        {
            // Arrange
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = "email_invalido.com";

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(message => message.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_INVALID));
        }
    }
}
