using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.IMAGES_FOR_RECIPES, "Add collumn on recipe table to save images")]
    public class Version0000005 : VersionBase
    {
        public override void Up()
        {
            Alter.Table("Recipes")
                .AddColumn("ImageIdentifier").AsString().Nullable();
        }
    }
}
