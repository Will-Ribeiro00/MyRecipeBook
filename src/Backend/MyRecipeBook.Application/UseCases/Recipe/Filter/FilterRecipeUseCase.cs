using AutoMapper;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Dto.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter
{
    public class FilterRecipeUseCase : IFilterRecipeUseCase
    {
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _repository;

        public FilterRecipeUseCase(IRecipeReadOnlyRepository repository, IMapper mapper, ILoggedUser loggedUser)
        {
            _repository = repository;
            _mapper = mapper;
            _loggedUser = loggedUser;
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
                Recipes = _mapper.Map<List<ResponseShortRecipeJson>>(recipes)
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
