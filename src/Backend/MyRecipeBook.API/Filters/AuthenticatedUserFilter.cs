using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses.Exception;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Exception.ExceptionsBase;
using MyRecipeBook.Exception;

namespace MyRecipeBook.API.Filters
{
    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {

        private readonly IAccessTokenValidator _accessTokenValidator;
        private readonly IUserReadOnlyRepository _repository;
        public AuthenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepository repository)
        {
            _accessTokenValidator = accessTokenValidator;
            _repository = repository;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = TokenOnRequest(context);

                var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);
                var exist = await _repository.ExistActiveUserWithIdentifier(userIdentifier);

                if (!exist)
                    throw new MyRecipeBookException(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("Token is expired")
                {
                    TokenIsExpired = true
                });
            }
            catch (MyRecipeBookException ex)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authentication))
            {
                throw new MyRecipeBookException(ResourceMessagesExceptions.WITHOUT_TOKEN);
            }

            return authentication["Bearer ".Length..].Trim();
        }
    }
}
