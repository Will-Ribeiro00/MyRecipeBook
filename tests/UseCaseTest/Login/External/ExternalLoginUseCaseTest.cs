using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Login.External;
using Shouldly;

namespace UseCaseTest.Login.External
{
    public class ExternalLoginUseCaseTest
    {
        [Fact]
        public async Task SuccessUserExist()
        {
            // Arrange
            (var user, var password) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            //Act
            var result = await useCase.Execute(user.Name, user.Email);

            // Assign 
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task SuccessUserDontExist()
        {
            // Arrange
            (var user, var password) = UserBuilder.Build();
            var useCase = CreateUseCase();

            //Act
            var result = await useCase.Execute(user.Name, user.Email);

            // Assign 
            result.ShouldNotBeNullOrEmpty();
        }

        private static ExternalLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var readRepository = new UserReadOnlyRepositoryBuilder().Build();
            var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            if (user is not null)
                readRepository.GetByEmail(user.Email);

            return new ExternalLoginUseCase(readRepository, writeRepository, accessTokenGenerator, unitOfWork);
        }
    }
}
