using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
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
        private readonly IBlobStorageService _blobStorageService;

        public DeleteRecipeUseCase(IRecipeWriteOnlyRepository writeRepository, IRecipeReadOnlyRepository readRepository, ILoggedUser loggedUser, IUnitOfWork unitOfWork, IBlobStorageService blobStorageService)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        public async Task Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipeExist = await _readRepository.RecipeExist(loggedUser, recipeId);
            if (!recipeExist)
                throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

            var recipe = await _readRepository.GetById(loggedUser, recipeId);
            if (!string.IsNullOrEmpty(recipe?.ImageIdentifier))
                await _blobStorageService.Delete(loggedUser, recipe.ImageIdentifier);

            await _writeRepository.Delete(recipeId);
            await _unitOfWork.Commit();
        }
    }
}
