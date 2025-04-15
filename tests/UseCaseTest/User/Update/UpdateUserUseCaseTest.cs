using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.User.Update
{
    public class UpdateUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);

            // Act
            await act.ShouldNotThrowAsync();

            // Assert
            user.Name.ShouldBe(request.Name);
            user.Email.ShouldBe(request.Email);
        }
        
        [Fact]
        public async Task ErrorNameEmpty()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;
            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);

            // Act
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.NAME_EMPTY);
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        [Fact]
        public async Task ErrorEmailAlreadyRegistered()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            var useCase = CreateUseCase(user, request.Email);

            Func<Task> act = async () => await useCase.Execute(request);

            // Act
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED);
            user.Name.ShouldNotBe(request.Name);
            user.Email.ShouldNotBe(request.Email);
        }

        private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var updateOnly = new UserUpdateOnlyRepositoryBuilder().GetUserById(user).Build();
            var readOnly = new UserReadOnlyRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();

            if (!string.IsNullOrWhiteSpace(email))
                readOnly.ExistActiveUserWithEmail(email);

            return new UpdateUserUseCase(loggedUser, updateOnly, readOnly.Build(), unitOfWork);
        }
    }
}
