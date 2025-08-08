namespace MyRecipeBook.Infrastructure.Migrations
{
    public abstract class DatabaseVersions
    {
        public const int TABLE_USER = 1;
        public const int ADJUSTE_TABLE_USER = 2;
        public const int ADD_COLUMN_USERIDENTIFIER = 3;
        public const int TABLE_RECIPES = 4;
        public const int IMAGES_FOR_RECIPES = 5;
        public const int TABLE_REFRESH_TOKEN = 6;
    }
}
