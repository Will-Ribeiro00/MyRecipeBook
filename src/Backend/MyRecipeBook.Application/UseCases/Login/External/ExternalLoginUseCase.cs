using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.External
{
    public class ExternalLoginUseCase : IExternalLoginUseCase
    {
        private readonly IUserReadOnlyRepository _readOnly;
        private readonly IUserWriteOnlyRepository _writeOnly;
        private readonly IAccessTokenGenerator _tokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalLoginUseCase(IUserReadOnlyRepository readOnly, IUserWriteOnlyRepository writeOnly, IAccessTokenGenerator tokenGenerator, IUnitOfWork unitOfWork)
        {
            _readOnly = readOnly;
            _writeOnly = writeOnly;
            _tokenGenerator = tokenGenerator;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Execute(string name, string email)
        {
            var user = await _readOnly.GetByEmail(email);

            if (user is null)
            {
                user = new Domain.Entities.User
                {
                    Name = name,
                    Email = email,
                    UserIdentifier = Guid.NewGuid(),
                    Password = "-"
                };

                await _writeOnly.Add(user);
                await _unitOfWork.Commit();
            }

            return _tokenGenerator.Generate(user.UserIdentifier);
        }
    }
}
