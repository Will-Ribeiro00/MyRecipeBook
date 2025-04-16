using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.User.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var password) = UserBuilder.Build();
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();

            request.Password = password;
            
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            await act.ShouldNotThrowAsync();

            // Assert
            user.Password.ShouldBe(passwordEncrypter.Encrypt(request.NewPassword));
        }

        [Fact]
        public async Task ErrorNewPasswordInvalid()
        {
            // Arrange
            (var user, var password) = UserBuilder.Build();
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();

            request.Password = password;
            request.NewPassword = "password-invalid";
            
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.PASSWORD_INVALID);
            user.Password.ShouldBe(passwordEncrypter.Encrypt(request.Password));
        }

        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var updateRepository = new UserUpdateOnlyRepositoryBuilder().GetUserById(user).Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var passwordEncrypter = PasswordEncrypterBuilder.Build();

            return new ChangePasswordUseCase(loggedUser,updateRepository, unitOfWork, passwordEncrypter);
        }
    }
}
