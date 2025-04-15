namespace MyRecipeBook.Domain.Secutiry.Cryptography
{
    public interface IPasswordEncrypter
    {
        public string Encrypt(string password);
    }
}
