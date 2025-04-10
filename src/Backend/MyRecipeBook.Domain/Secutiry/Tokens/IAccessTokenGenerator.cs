namespace MyRecipeBook.Domain.Secutiry.Tokens
{
    public interface IAccessTokenGenerator
    {
        public string Generate(Guid userIdentifier);
    }
}
