using FluentValidation;
using FluentValidation.Validators;
using MyRecipeBook.Exception;
using System.Text;
using System.Text.RegularExpressions;

namespace MyRecipeBook.Application.UseCases.User
{
    public partial class PasswordValidator<T> : PropertyValidator<T, string>
    {
        private const string ERROR_MESSAGE_KEY = "Error";

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return $"{{{ERROR_MESSAGE_KEY}}}";
        }

        public override string Name => "PasswordValidator";

        public override bool IsValid(ValidationContext<T> context, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            if (password.Length < 8)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            if (!UpperCaseLetter().IsMatch(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            if (!LowerCaseLetter().IsMatch(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            if (!Numbers().IsMatch(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            if (!SpecialSymbols().IsMatch(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceMessagesExceptions.PASSWORD_INVALID);
                return false;
            }

            return true;
        }


        [GeneratedRegex(@"^(?=.*[A-Z]).+$")]
        private static partial Regex UpperCaseLetter();
        [GeneratedRegex(@"^(?=.*[a-z]).+$")]
        private static partial Regex LowerCaseLetter();
        [GeneratedRegex(@"^(?=.*\d).+$")]
        private static partial Regex Numbers();
        [GeneratedRegex(@"^(?=.*[\W_]).+$")]
        private static partial Regex SpecialSymbols();
    }
}
