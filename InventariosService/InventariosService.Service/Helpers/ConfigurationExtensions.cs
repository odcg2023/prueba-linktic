
using InventariosService.Common;

namespace InventariosService.Service.Helpers
{
    public static class ConfigurationExtensions
    {
        public static string GetSecureConnectionString(this IConfiguration configuration, string connectionName)
        {
            var connectionTemplate = configuration.GetConnectionString(connectionName);

            return connectionTemplate
                .Replace("[SERVER]", Crypto.Decrypt(configuration["Levels:Level1"]))
                .Replace("[DATABASE]", Crypto.Decrypt(configuration["Levels:Level2"]))
                .Replace("[USER]", Crypto.Decrypt(configuration["Levels:Level3"]))
                .Replace("[PASSWORD]", Crypto.Decrypt(configuration["Levels:Level4"]));
        }
    }
}
