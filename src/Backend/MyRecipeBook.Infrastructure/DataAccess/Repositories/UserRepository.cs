using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository, IUserUpdateOnlyRepository
    {
        private readonly MyRecipeBookDbContext _context;

        public UserRepository(MyRecipeBookDbContext context) => _context = context;

        public async Task Add(User user) => await _context.Users.AddAsync(user);

        public async Task<bool> ExistActiveUserWithEmail(string email) => await _context.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);

        public async Task<User?> GetUserByEmailAndPassword(string email, string password) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Active && user.Email.Equals(email) && user.Password.Equals(password));
        public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier) => await _context.Users.AnyAsync(user => user.UserIdentifier.Equals(userIdentifier) && user.Active);

        public async Task<User> GetUserById(long id) => await _context.Users.FirstAsync(user => user.Id == id);

        public void Update(User user) => _context.Users.Update(user);
    }
}
