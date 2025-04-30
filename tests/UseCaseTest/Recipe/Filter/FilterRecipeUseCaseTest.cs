using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.FilterAll;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.Filter
{
    public class FilterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestFilterRecipeJsonBuilder.Build();
            var recipes = RecipeBuilder.Collection(user);
            var useCase = CreateUseCase(user, recipes);

            // Act
            var result = await useCase.Execute(request);

            // Assert
            result.ShouldNotBeNull();
            result.Recipes.ShouldNotBeNull();
            result.Recipes.ShouldNotBeEmpty();
            result.Recipes.Count.ShouldBe(recipes.Count);
        }

        [Fact]
        public async Task ErrorCookingTimeInvalid()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);
            var recipes = RecipeBuilder.Collection(user);
            var useCase = CreateUseCase(user, recipes);

            Func<Task> act = async () => await useCase.Execute(request);

            // Act
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED);
        }

        private static FilterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var repository = new RecipeReadOnlyRepositoryBuilder().Filter(user, recipes).Build();

            return new FilterRecipeUseCase(repository, mapper, loggedUser);
        }
    }
}
