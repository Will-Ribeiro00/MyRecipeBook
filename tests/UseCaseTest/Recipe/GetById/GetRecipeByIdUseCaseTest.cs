using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.FilterById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.GetById
{
    public class GetRecipeByIdUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var useCase = CreateUseCase(user, recipe);

            // Act
            var result = await useCase.Execute(recipe.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNullOrWhiteSpace();
            result.Title.ShouldBe(recipe.Title);
            result.ImageUrl.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ErrorRecipeNotFound()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipeId: 1000);

            var result = await act.ShouldThrowAsync<NotFoundException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_NOT_FOUND);
        }

        private static GetRecipeByIdUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var repository = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

            return new GetRecipeByIdUseCase(repository, mapper, loggedUser, blobStorage);
        }
    }
}
