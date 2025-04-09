using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IUserReadOnlyRepository
    {
        public Task<bool> ExistActiveUserWithEmail(string email);
        public Task<Entities.User?> GetUserByEmailAndPassword(string email, string password);
    }
}
