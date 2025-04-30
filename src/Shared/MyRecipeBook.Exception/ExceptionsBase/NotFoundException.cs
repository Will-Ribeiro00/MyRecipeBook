namespace MyRecipeBook.Exception.ExceptionsBase
{
    public class NotFoundException : MyRecipeBookException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
