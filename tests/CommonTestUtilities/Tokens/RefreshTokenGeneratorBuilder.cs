using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Tokens
{
    public class RefreshTokenGeneratorBuilder
    {
        public static IRefreshTokenGenerator Build() => new RefreshTokenGenerator();
    }
}
