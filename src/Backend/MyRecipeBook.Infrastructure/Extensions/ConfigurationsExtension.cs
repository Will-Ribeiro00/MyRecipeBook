using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions
{
    public static class ConfigurationsExtension
    {
        public static bool IsUnitTestEnviroment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }

        public static DatabaseType DatabaseType(this IConfiguration configurarion)
        {
            var databaseType = configurarion.GetConnectionString("DatabaseType");

            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);
        }
        public static string ConnectionString(this IConfiguration configuration)
        {
            var databaseType = configuration.DatabaseType();

            if (databaseType == Domain.Enums.DatabaseType.MySql)
                return configuration.GetConnectionString("DefaultConnectionMySql")!;
            else
                return configuration.GetConnectionString("DefaultConnectionSqlServer")!;
        }
    }
}
