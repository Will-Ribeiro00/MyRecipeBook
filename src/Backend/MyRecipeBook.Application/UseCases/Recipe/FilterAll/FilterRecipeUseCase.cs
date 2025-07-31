using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Dto.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.FilterAll
{
    public class FilterRecipeUseCase : IFilterRecipeUseCase
    {
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _repository;
        private readonly IBlobStorageService _blobStorageService; 

        public FilterRecipeUseCase(IRecipeReadOnlyRepository repository, IMapper mapper, ILoggedUser loggedUser, IBlobStorageService blobStorageService)
        {
            _repository = repository;
            _mapper = mapper;
            _loggedUser = loggedUser;
            _blobStorageService = blobStorageService;
        }

        public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.User();

            var filters = new FilterRecipesDto()
            {
                RecipeTitle_Ingredient = request.RecipeTitle_Ingredient,
                CookingTimes = [.. request.CookingTimes.Distinct().Select(c => (Domain.Enums.CookingTime)c)],
                Difficulties = [.. request.Difficulties.Distinct().Select(d => (Domain.Enums.Difficulty)d)],
                DishTypes = [.. request.DishTypes.Distinct().Select(d => (Domain.Enums.DishType)d)]
            };

            var recipes = await _repository.Filter(loggedUser, filters);

            return new ResponseRecipesJson
            {
                Recipes = await recipes.MapToShortRecipeJson(loggedUser, _blobStorageService, _mapper)
            };
        }

        public static void Validate(RequestFilterRecipeJson request)
        {
            var result = new FilterRecipeValidator().Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
