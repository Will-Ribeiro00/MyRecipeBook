using Bogus;
using MyRecipeBook.Communication.Requests.User;

namespace CommonTestUtilities.Requests
{
    public class RequestChangePasswordJsonBuilder
    {
        public static RequestChangePasswordJson Build()
        {
            return new Faker<RequestChangePasswordJson>()
                .RuleFor(user => user.Password, (f) => f.Internet.Password())
                .RuleFor(user => user.NewPassword, (f) => f.Internet.Password(length: 8, prefix: "!Aa1"));
        }
    }
}
