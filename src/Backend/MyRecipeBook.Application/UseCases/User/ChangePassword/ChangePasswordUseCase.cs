﻿using FluentValidation.Results;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypter _passwordEncrypter;

        public ChangePasswordUseCase(ILoggedUser loggedUser, IUserUpdateOnlyRepository repository, IUnitOfWork unitOfWork, IPasswordEncrypter passwordEncrypter)
        {
            _loggedUser = loggedUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
        }

        public async Task Execute(RequestChangePasswordJson request)
        {
            var loggedUser = await _loggedUser.User();

            Validate(request, loggedUser);

            var user = await _repository.GetUserById(loggedUser.Id);

            user.Password = _passwordEncrypter.Encrypt(request.NewPassword);

            _repository.Update(user);
            await _unitOfWork.Commit();
        }

        private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
        {
            var result = new ChangePasswordValidator().Validate(request);

            if (!_passwordEncrypter.IsValid(request.Password, loggedUser.Password))
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesExceptions.PASSWORD_DIFFERENT_CURRENT_PASSWORD));

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
