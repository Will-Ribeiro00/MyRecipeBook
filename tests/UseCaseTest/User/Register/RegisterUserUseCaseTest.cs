using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.User.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase();

            // Act
            var result = await useCase.Execute(request);

            // Arrange
            result.ShouldNotBeNull();
            result.Name.ShouldBe(request.Name);
        }
        [Fact]
        public async Task ErrorEmailAlreadyRegistered()
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase(request.Email);

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Arrange
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED);
        }
        [Fact]
        public async Task ErrorNameEmpty()
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;
            var useCase = CreateUseCase();

            // Act
            Func<Task> act = async () => await useCase.Execute(request);
            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Arrange
            result.ErrorMessages.ShouldHaveSingleItem();
            result.ErrorMessages.ShouldContain(ResourceMessagesExceptions.NAME_EMPTY);
        }

        private static RegisterUserUseCase CreateUseCase(string? email = null)
        {
            var mapper = MapperBuilder.Build();
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var writeOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();
            var readOnlyRepository = new UserReadOnlyRepositoryBuilder();

            if (!string.IsNullOrWhiteSpace(email))
                readOnlyRepository.ExistActiveUserWithEmail(email);

            return new RegisterUserUseCase(readOnlyRepository.Build(), writeOnlyRepository, mapper, passwordEncrypter, unitOfWork);
        }
    }
}
