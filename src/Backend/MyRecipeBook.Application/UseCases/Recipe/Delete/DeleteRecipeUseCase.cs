using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete
{
    public class DeleteRecipeUseCase : IDeleteRecipeUseCase
    {
        private readonly IRecipeWriteOnlyRepository _writeRepository;
        private readonly IRecipeReadOnlyRepository _readRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRecipeUseCase(IRecipeWriteOnlyRepository writeRepository, IRecipeReadOnlyRepository readRepository, ILoggedUser loggedUser, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _readRepository.RecipeExist(loggedUser, recipeId);
            if (!recipe)
                throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

            await _writeRepository.Delete(recipeId);
            await _unitOfWork.Commit();
        }
    }
}
