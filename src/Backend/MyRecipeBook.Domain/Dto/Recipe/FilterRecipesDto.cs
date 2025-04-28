using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Dto.Recipe
{
    public record FilterRecipesDto
    {
        public string? RecipeTitle_Ingredient { get; set; }
        public IList<CookingTime> CookingTimes { get; set; } = [];
        public IList<Difficulty> Difficulties { get; set; } = [];
        public IList<DishType> DishTypes { get; set; } = [];
    }
}
