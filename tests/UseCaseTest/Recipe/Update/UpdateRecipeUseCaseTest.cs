using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.Update
{
    public class UpdateRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var request = RequestRecipeJsonBuilder.Build();
            var useCase = CreateUseCase(user, recipe);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

            // Assert
            await act.ShouldNotThrowAsync();
        }

        [Fact]
        public async Task ErrorRecipeNotFound()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var request = RequestRecipeJsonBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipeId: 1000, request);
            var response = await act.ShouldThrowAsync<NotFoundException>();

            // Assert
            response.GetErrorMessages().ShouldHaveSingleItem();
            response.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_NOT_FOUND);
        }

        [Fact]
        public async Task ErrorTitleEmpty()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipeId: 1000, request);
            var response = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            response.GetErrorMessages().ShouldHaveSingleItem();
            response.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY);
        }

        private static UpdateRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new UpdateRecipeUseCase(repository, loggedUser, mapper, unitOfWork);
        }
    }
}
