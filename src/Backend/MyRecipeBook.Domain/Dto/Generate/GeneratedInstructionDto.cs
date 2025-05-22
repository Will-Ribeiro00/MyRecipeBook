namespace MyRecipeBook.Domain.Dto.Generate
{
    public record GeneratedInstructionDto
    {
        public int Step { get; init; }
        public string Description { get; init; } = string.Empty;
    }
}
