namespace SharedKernel.Runtime
{
    public interface IExceptionHandler
    {
        Task PutToDatabaseAsync(Exception ex);
    }
}
