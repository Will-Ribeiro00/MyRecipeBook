namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IUserUpdateOnlyRepository
    {
        public Task<Entities.User> GetUserById(long id);
        public void Update(Entities.User user);
    }
}
