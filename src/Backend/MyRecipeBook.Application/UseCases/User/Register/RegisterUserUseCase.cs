using AutoMapper;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ITokenRepository _tokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public RegisterUserUseCase(IUserReadOnlyRepository readOnly, IUserWriteOnlyRepository writeOnly, IMapper mapper, IPasswordEncrypter passwordEncrypter, IUnitOfWork unitOfWork, IAccessTokenGenerator accessTokenGenerator, ITokenRepository tokenRepository, IRefreshTokenGenerator refreshTokenGenerator)
        {
            _readOnlyRepository = readOnly;
            _writeOnlyRepository = writeOnly;
            _mapper = mapper;
            _passwordEncrypter = passwordEncrypter;
            _unitOfWork = unitOfWork;
            _accessTokenGenerator = accessTokenGenerator;
            _tokenRepository = tokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            await Validate(request);

            var user = _mapper.Map<Domain.Entities.User>(request);
            user.Password = _passwordEncrypter.Encrypt(request.Password);
            user.UserIdentifier = Guid.NewGuid();

            await _writeOnlyRepository.Add(user);
            await _unitOfWork.Commit();

            var refreshToken = await CreateAndSaveRefreshToken(user);

            return new ResponseRegisteredUserJson
            {
                Name = request.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                    RefreshToken = refreshToken
                }
            };
        }

        private async Task Validate(RequestRegisterUserJson request)
        {
            var validator = new RegisterUserValidator();

            var result = validator.Validate(request);

            var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (emailExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(erro => erro.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            var refreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = user.Id,
            };

            await _tokenRepository.SaveNewRefreshToken(refreshToken);

            await _unitOfWork.Commit();

            return refreshToken.Value;
        }
    }
}
