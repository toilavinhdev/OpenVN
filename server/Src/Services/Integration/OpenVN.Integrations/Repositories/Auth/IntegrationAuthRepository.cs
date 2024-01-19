using SharedKernel.MySQL;

namespace OpenVN.BackgroundJob
{
    public class IntegrationAuthRepository : IIntegrationAuthRepository
    {
        private readonly IServiceProvider _provider;

        public IntegrationAuthRepository(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task WriteSignInAsync(SignInHistory history, CancellationToken cancellationToken)
        {
            var properties = history.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
            var columns = string.Join(", ", properties.Select(p => $"`{p.Name}`"));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var cmd = @$"INSERT INTO {new SignInHistory().GetTableName()} ( {columns} ) VALUES ( {parameters} );";

            using (var dbConnection = new DbConnection())
            {
                await dbConnection.ExecuteAsync(cmd, history);
                await dbConnection.CommitAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
