namespace MyRecipeBook.Domain.Secutiry.Tokens
{
    public interface IRefreshTokenGenerator
    {
        public string Generate();
    }
}
