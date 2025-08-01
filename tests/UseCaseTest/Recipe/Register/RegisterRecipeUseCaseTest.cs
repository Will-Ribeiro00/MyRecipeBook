using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;
using UseCaseTest.Recipe.InlineDatas;

namespace UseCaseTest.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task SuccessWithoutImage()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestRegisterRecipeFormDataBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            var result = await useCase.Execute(request);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNullOrWhiteSpace();
            result.Title.ShouldBe(request.Title);
        }

        [Theory]
        [ClassData(typeof(ImageTypesInlineData))]
        public async Task SuccessWithImage(IFormFile file)
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var request = RequestRegisterRecipeFormDataBuilder.Build(file);
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
            var request = RequestRegisterRecipeFormDataBuilder.Build();
            request.Title = string.Empty;
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result =  await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY);
        }

        [Fact]
        public async Task ErrorInvalidFile()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var txtFile = FormFileBuilder.Txt();
            var request = RequestRegisterRecipeFormDataBuilder.Build(txtFile);
            var useCase = CreateUseCase(user);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.ONLY_IMAGES_ACCEPTED);
        }

        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var repository = RecipeWriteOnlyRecipositoryBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();
            var mapper = MapperBuilder.Build();
            var blobStorage = new BlobStorageServiceBuilder().Build();

            return new RegisterRecipeUseCase(repository, loggedUser, unitOfWork, mapper, blobStorage);
        }
    }
}
