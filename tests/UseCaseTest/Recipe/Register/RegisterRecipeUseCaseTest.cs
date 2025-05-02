using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestRecipeJsonBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            var result = await useCase.Execute(request);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNullOrWhiteSpace();
            result.Title.ShouldBe(request.Title);
        }

        [Fact]
        public async Task ErrorTitleEmpty()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result =  await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY);
        }

        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var repository = RecipeWriteOnlyRecipositoryBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();
            var mapper = MapperBuilder.Build();
            return new RegisterRecipeUseCase(repository, loggedUser, unitOfWork, mapper);
        }
    }
}
