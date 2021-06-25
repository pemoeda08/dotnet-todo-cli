using Microsoft.Extensions.Configuration;

namespace TodoCli
{
    public class AppSettings
    {
        public string ApiRepositoryBaseUrl { get; init; }
    }

    public static class TodoAppConfig
    {
        private static AppSettings _instance = null;
        public static AppSettings Instance()
        {
            if (_instance == null)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                _instance = configuration.Get<AppSettings>();
            }

            return _instance;
        }
    }
}