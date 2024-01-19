using Microsoft.Extensions.Configuration;
using Serilog;

namespace SharedKernel.Core
{
    public static class CoreSettings
    {
        public static readonly bool IsSingleDevice = false;

        public static Dictionary<string, string> ConnectionStrings { get; private set; }

        public static DefaultLoggingConfig DefaultLoggingConfig { get; private set; }

        public static DefaultEmailConfig DefaultEmailConfig { get; private set; }

        public static DefaultElasticSearchConfig DefaultElasticSearchConfig { get; private set; }

        public static DefaultS3Config DefaultS3Config { get; private set; }

        public static List<string> Black3pKeywords { get; private set; }

        public static void SetConfig(IConfiguration configuration, ILogger logger)
        {
            SetConnectionStrings(configuration);
            SetLoggingConfig(configuration, logger);
            SetEmailConfig(configuration);
            SetElasticSearchConfig(configuration);
            SetS3Config(configuration);
            SetBlack3pKeywords(configuration);
        }

        public static void SetConnectionStrings(IConfiguration configuration)
        {
            ConnectionStrings = configuration.GetRequiredSection("ConnectionStrings").Get<Dictionary<string, string>>();
        }

        public static void SetEmailConfig(IConfiguration configuration)
        {
            DefaultEmailConfig.SetConfig(configuration);
        }

        public static void SetLoggingConfig(IConfiguration configuration, ILogger logger)
        {
            DefaultLoggingConfig.SetConfig(configuration, logger);
        }

        public static void SetElasticSearchConfig(IConfiguration configuration)
        {
            DefaultElasticSearchConfig.SetConfig(configuration);
        }

        public static void SetS3Config(IConfiguration configuration)
        {
            DefaultS3Config.SetConfig(configuration);
        }

        public static void SetBlack3pKeywords(IConfiguration configuration)
        {
            Black3pKeywords = configuration.GetValue<string>("Black3pKeyword").Split(",").ToList();
        }
    }
}
