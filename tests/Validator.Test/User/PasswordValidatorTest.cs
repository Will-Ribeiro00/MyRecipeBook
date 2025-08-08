using FluentValidation;
using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Communication.Requests.User;
using Shouldly;

namespace Validator.Test.User
{
    public class PasswordValidatorTest
    {
        [Theory]
        [InlineData("")] // vazio
        [InlineData("   ")] // Espaços em branco
        [InlineData(null)] // Nula
        [InlineData("a")] // length < 8
        [InlineData("ab")] // length < 8
        [InlineData("abc")] // length < 8
        [InlineData("abcd")] // length < 8
        [InlineData("abcde")] // length < 8
        [InlineData("abcdef")] // length < 8
        [InlineData("abcdefg")] // length < 8 
        [InlineData("abcdefgh")] // sem letra maiúscula
        [InlineData("ABCDEFGH")] // sem letra minúscula
        [InlineData("Abcdefgh_")] // sem número
        [InlineData("Abcdefgh1")] // sem caracter especial
        public void ErrorsPasswordInvalid(string password)
        {
            //Arrange
            var validator = new PasswordValidator<RequestRegisterUserJson>();

            //Act
            var result = validator.IsValid(new ValidationContext<RequestRegisterUserJson>(new RequestRegisterUserJson()), password: password);

            //Arrange
            result.ShouldBeFalse();
        }
    }
}
