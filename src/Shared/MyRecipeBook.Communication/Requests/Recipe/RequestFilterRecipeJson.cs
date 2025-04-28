using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requests.Recipe
{
    public class RequestFilterRecipeJson
    {
        public string? RecipeTitle_Ingredient { get; init; }
        public IList<CookingTime> CookingTimes { get; init; } = [];
        public IList<Difficulty> Difficulties { get; init; } = [];
        public IList<DishType> DishTypes { get; init; } = [];
    }
}
