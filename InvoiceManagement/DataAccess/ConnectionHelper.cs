using Microsoft.Extensions.Configuration;

namespace InvoiceManagement.DataAccess
{
    public static class ConnectionHelper
    {
        public static string Get()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();
            return config.GetConnectionString("DefaultConnection");
        }
    }
}
