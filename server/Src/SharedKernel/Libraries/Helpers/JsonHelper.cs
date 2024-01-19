using Microsoft.Extensions.Configuration;

namespace SharedKernel.Libraries
{
    public static class JsonHelper
    {
        public static object _lockObj = new object();
        public static Dictionary<string, IConfigurationRoot> Pairs = new Dictionary<string, IConfigurationRoot>();

        public static IConfigurationRoot GetConfiguration(string jsonFileName = "appsettings.json")
        {
            lock (_lockObj)
            {
                if (!Pairs.ContainsKey(jsonFileName))
                {
                    Pairs.Add(jsonFileName, new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile(jsonFileName, optional: false, reloadOnChange: true)
                                                .AddEnvironmentVariables()
                                                .Build()
                             );
                }
                return Pairs[jsonFileName];
            }
        }

    }
}
