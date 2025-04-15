using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserUpdateOnlyRepositoryBuilder
    {
        private readonly Mock<IUserUpdateOnlyRepository> _repository;

        public UserUpdateOnlyRepositoryBuilder() => _repository = new Mock<IUserUpdateOnlyRepository>();
        public IUserUpdateOnlyRepository Build() => _repository.Object;

        public UserUpdateOnlyRepositoryBuilder GetUserById(User user)
        {
            _repository.Setup(repository => repository.GetUserById(user.Id)).ReturnsAsync(user);
            return this;
        }
    }
}
