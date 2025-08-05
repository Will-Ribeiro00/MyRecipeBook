using MyRecipeBook.Domain.Secutiry.Tokens;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Refresh
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
