using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.Delete
{
    public class DeleteRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var useCase = CreateUseCase(user, recipe);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipe.Id);

            // Assert
            await act.ShouldNotThrowAsync();
        }

        [Fact]
        public async Task ErrorRecipeNotFound()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipeId: 1000);
            var response = await act.ShouldThrowAsync<NotFoundException>();

            // Assert
            response.GetErrorMessages().ShouldHaveSingleItem();
            response.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_NOT_FOUND);
        }

        private static DeleteRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var readRepository = new RecipeReadOnlyRepositoryBuilder().RecipeExist(user, recipe).Build();
            var writeRepository = RecipeWriteOnlyRecipositoryBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new DeleteRecipeUseCase(writeRepository, readRepository, loggedUser, unitOfWork);
        }
    }
}
