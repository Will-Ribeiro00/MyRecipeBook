using FluentMigrator;
namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.ADD_COLUMN_USERIDENTIFIER, "Add column userIdentifier and populate existing users")]
    public class Version0000003 : VersionBase
    {
        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("UserIdentifier").AsGuid().Nullable();

            Execute.Sql("UPDATE Users SET UserIdentifier = UUID() WHERE UserIdentifier IS NULL");

            Alter.Column("UserIdentifier").OnTable("Users").AsGuid().NotNullable();
        }
    }
}
