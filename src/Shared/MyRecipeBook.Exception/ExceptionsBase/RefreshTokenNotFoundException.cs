using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase
{
    public class RefreshTokenNotFoundException : MyRecipeBookException
    {
        public RefreshTokenNotFoundException() : base(ResourceMessagesExceptions.INVALID_SESSION) { }
        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
