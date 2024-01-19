using Microsoft.Extensions.Configuration;

namespace SharedKernel.Providers
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        private readonly IConfiguration _configuration;

        public ApplicationConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T GetConfiguration<T>(string key = "") where T : class
        {
            var instance = Activator.CreateInstance(typeof(T));
            _configuration.GetRequiredSection(!string.IsNullOrEmpty(key) ? key : typeof(T).Name).Bind(instance);

            return (T)instance;
        }
    }
}
