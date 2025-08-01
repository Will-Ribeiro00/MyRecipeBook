using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;
using UseCaseTest.Recipe.InlineDatas;

namespace UseCaseTest.Recipe.Image
{
    public class AddUpdateImageCoverUseCaseTest
    {
        [Theory]
        [ClassData(typeof(ImageTypesInlineData))]
        public async Task Success(IFormFile file)
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var useCase = CreateUseCase(user, recipe);

            // Act
            Func<Task> act = async () => await useCase.Execute(recipe.Id, file);

            // Assert
            await act.ShouldNotThrowAsync();
        }

        [Theory]
        [ClassData(typeof(ImageTypesInlineData))]
        public async Task ErrorImageNotFound(IFormFile file)
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(1, file);
            var result = await act.ShouldThrowAsync<NotFoundException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_NOT_FOUND);
        }

        [Fact]
        public async Task ErrorFileIsTxt()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var recipe = RecipeBuilder.Build(user);
            var file = FormFileBuilder.Txt();
            var useCase = CreateUseCase(user, recipe);

            // Act
            Func<Task> act = async () => await useCase.Execute(1, file);
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED);
        }

        private static AddUpdateImageCoverUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
            var loggedUser = LoggedUserBuilder.Build(user); 
            var unitOfWork = UnitOfWorkBuilder.Build();
            var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user,recipe?.ImageIdentifier).Build();

            return new AddUpdateImageCoverUseCase(repository, loggedUser, unitOfWork, blobStorage);
        }
    }
}
