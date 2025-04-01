using FluentValidation;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Exception;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        public RegisterUserValidator()
        {
            RuleFor(user => user.Name).NotEmpty()
                .WithMessage(ResourceMessagesExceptions.NAME_EMPTY);

            RuleFor(user => user.Email).NotEmpty()
                .WithMessage(ResourceMessagesExceptions.EMAIL_EMPTY);

            RuleFor(user => user.Email).EmailAddress()
                .WithMessage(ResourceMessagesExceptions.EMAIL_INVALID);

            
            RuleFor(user => user.Password)
                .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                        .WithMessage(ResourceMessagesExceptions.PASSWORD_INVALID)
                    .MinimumLength(8)
                        .WithMessage(ResourceMessagesExceptions.PASSWORD_INVALID)
                    .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$")
                        .WithMessage(ResourceMessagesExceptions.PASSWORD_INVALID);
        }
    }
}
