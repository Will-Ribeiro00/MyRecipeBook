using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.ServiceBus;
using MyRecipeBook.Application.UseCases.User.Delete.Request;
using Shouldly;

namespace UseCaseTest.User.Delete.Request
{
    public class RequestDeleteUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute();
            await act.ShouldNotThrowAsync();

            // Assert
            user.Active.ShouldBeFalse();
        }

        private static RequestDeleteUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var queue = DeleteUserQueueBuilder.Build();
            var repository = new UserUpdateOnlyRepositoryBuilder().GetUserById(user).Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new RequestDeleteUserUseCase(queue, repository, loggedUser, unitOfWork);
        }
    }
}
