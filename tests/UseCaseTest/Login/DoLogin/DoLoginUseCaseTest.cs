using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Login;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Login.DoLogin
{
    public class DoLoginUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var password) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            //Act
            var result = await useCase.Execute(new RequestLoginJson
            {
                Email = user.Email,
                Password = password
            });

            // Assign 
            result.ShouldNotBeNull();
            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Name.ShouldBe(user.Name);
        }
        [Fact]
        public async Task ErrorInvalidUser()
        {
            // Arrange
            var request = RequestLoginJsonBuilder.Build();
            var useCase = CreateUseCase();
            Func<Task> act = act = async () => await useCase.Execute(request);

            // Act and Arrange
            var result = await act.ShouldThrowAsync<InvalidLoginException>();
            result.Message.ShouldBe(ResourceMessagesExceptions.EMAIL_OR_PASSWORD_INVALID);
        }

        private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var readOnlyRepository = new UserReadOnlyRepositoryBuilder();

            if (user is not null)
                readOnlyRepository.GetUserByEmailAndPassword(user);

            return new DoLoginUseCase(readOnlyRepository.Build(), passwordEncrypter);
        }
    }
}
