using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator;

namespace CommonTestUtilities.Tokens
{
    public class JwtTokenGeneratorBuilder
    {
        public static IAccessTokenGenerator Build() => new JwtTokenGenerator(5, "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6");
    }
}
