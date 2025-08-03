using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using Shouldly;

namespace UseCaseTest.User.Delete.Delete
{
    public class DeleteUserAccountUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var useCase = CreateUseCase();

            // Act and Assert
            Func<Task> act = async () => await useCase.Execute(user.UserIdentifier);
            await act.ShouldNotThrowAsync();
        }

        private static DeleteUserAccountUseCase CreateUseCase()
        {
            var repository = UserDeleteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var blobStorage = new BlobStorageServiceBuilder().Build();

            return new DeleteUserAccountUseCase(repository, unitOfWork, blobStorage);
        }
    }
}
