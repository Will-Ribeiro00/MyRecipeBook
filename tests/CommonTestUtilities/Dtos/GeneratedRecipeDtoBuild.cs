using Bogus;
using MyRecipeBook.Domain.Dto.Generate;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.Dtos
{
    public class GeneratedRecipeDtoBuild
    {
        public static GeneratedRecipeDto Build()
        {
            return new Faker<GeneratedRecipeDto>()
                .RuleFor(r => r.Title, faker => faker.Lorem.Word())
                .RuleFor(r => r.CookingTime, faker => faker.PickRandom<CookingTime>())
                .RuleFor(r => r.Ingredients, faker => faker.Make(1, () => faker.Commerce.ProductName()))
                .RuleFor(r => r.Instructions, faker => faker.Make(1, () => new GeneratedInstructionDto
                {
                    Step = 1,
                    Description = faker.Lorem.Paragraph()
                }));
        }
    }
}
