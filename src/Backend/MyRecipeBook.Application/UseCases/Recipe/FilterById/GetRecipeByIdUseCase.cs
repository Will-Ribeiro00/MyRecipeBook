using AutoMapper;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.FilterById
{
    public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
    {
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _repository;
        private readonly IBlobStorageService _blobStorageService;
        public GetRecipeByIdUseCase(IRecipeReadOnlyRepository repository, IMapper mapper, ILoggedUser loggedUser, IBlobStorageService blobStorageService)
        {
            _repository = repository;
            _mapper = mapper;
            _loggedUser = loggedUser;
            _blobStorageService = blobStorageService;
        }
        public async Task<ResponseRecipeJson> Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _repository.GetById(loggedUser, recipeId) ?? throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

            var response = _mapper.Map<ResponseRecipeJson>(recipe);

            if (!string.IsNullOrEmpty(recipe.ImageIdentifier))
            {
                var url = await _blobStorageService.GetFileUrl(loggedUser, recipe.ImageIdentifier);

                response.ImageUrl = url;
            }

            return response;
        }
    }
}
