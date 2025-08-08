using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncrypterBuilder
    {
        public static IPasswordEncrypter Build() => new BCryptNet();
    }
}
