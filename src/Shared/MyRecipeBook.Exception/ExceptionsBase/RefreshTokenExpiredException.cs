using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase
{
    public class RefreshTokenExpiredException : MyRecipeBookException
    {
        public RefreshTokenExpiredException() : base(ResourceMessagesExceptions.EXPIRED_SESSION) { }
        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
