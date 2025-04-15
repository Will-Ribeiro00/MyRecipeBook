
using FluentValidation.Results;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _updateOnlyrepository;
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserUseCase(ILoggedUser loggedUser, IUserUpdateOnlyRepository updateOnlyrepository, IUserReadOnlyRepository readOnlyRepository, IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _updateOnlyrepository = updateOnlyrepository;
            _readOnlyRepository = readOnlyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Execute(RequestUpdateUserJson request)
        {
            var loggedUser = await _loggedUser.User();

            await Validate(request, loggedUser.Email);

            var user = await _updateOnlyrepository.GetUserById(loggedUser.Id);

            user.Name = request.Name;
            user.Email = request.Email;

            _updateOnlyrepository.Update(user);
            await _unitOfWork.Commit();
        }

        private async Task Validate(RequestUpdateUserJson request, string currentEmail)
        {
            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            if (!currentEmail.Equals(request.Email))
            {
                var userExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);
                if (userExist)
                    result.Errors.Add(new ValidationFailure("email", ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
            }

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
