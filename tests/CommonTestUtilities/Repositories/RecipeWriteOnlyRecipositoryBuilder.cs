using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories
{
    public class RecipeWriteOnlyRecipositoryBuilder
    {
        public static IRecipeWriteOnlyRepository Build() => new Mock<IRecipeWriteOnlyRepository>().Object;
    }
}
