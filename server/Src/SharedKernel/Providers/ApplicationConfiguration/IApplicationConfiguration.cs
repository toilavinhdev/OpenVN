namespace SharedKernel.Providers
{
    public interface IApplicationConfiguration
    {
        T GetConfiguration<T>(string key = "") where T : class;
    }
}
