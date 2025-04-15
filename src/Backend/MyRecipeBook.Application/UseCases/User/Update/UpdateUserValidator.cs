using FluentValidation;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Exception;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
    {
        public UpdateUserValidator()
        {
            RuleFor(request => request.Name).NotEmpty()
                .WithMessage(ResourceMessagesExceptions.NAME_EMPTY);

            RuleFor(request => request.Email).NotEmpty()
                .WithMessage(ResourceMessagesExceptions.EMAIL_EMPTY);

            When(request => !string.IsNullOrWhiteSpace(request.Email), () =>
            {
                RuleFor(request => request.Email).EmailAddress()
                    .WithMessage(ResourceMessagesExceptions.EMAIL_INVALID);
            });
        }
    }
}
