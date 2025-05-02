using AutoMapper;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace MyRecipeBook.Application.UseCases.Dashboard.Get
{
    public class GetDashboardUseCase : IGetDashboardUseCase
    {
        private readonly IRecipeReadOnlyRepository _repository;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;

        public GetDashboardUseCase(IRecipeReadOnlyRepository repository, IMapper mapper, ILoggedUser loggedUser)
        {
            _repository = repository;
            _loggedUser = loggedUser;
            _mapper = mapper;
        }

        public async Task<ResponseRecipesJson> Execute()
        {
            var loggedUser = await _loggedUser.User();

            var recipes = await _repository.GetForDashboard(loggedUser);

            return new ResponseRecipesJson
            {
                Recipes = _mapper.Map<IList<ResponseShortRecipeJson>>(recipes)
            };
        }
    }
}
