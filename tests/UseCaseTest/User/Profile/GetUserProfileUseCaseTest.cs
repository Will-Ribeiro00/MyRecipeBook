using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using MyRecipeBook.Application.UseCases.User.Profile;
using Shouldly;

namespace UseCaseTest.User.Profile
{
    public class GetUserProfileUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, var _) = UserBuilder.Build();
            var useCase = CreateUseCase(user);

            // Act
            var result = await useCase.Execute();

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(user.Name); 
            result.Email.ShouldBe(user.Email);
        }

        private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);

            return new GetUserProfileUseCase(loggedUser, mapper);
        }
    }
}
