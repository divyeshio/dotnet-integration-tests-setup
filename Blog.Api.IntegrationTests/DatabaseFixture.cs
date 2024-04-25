using Npgsql;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace Blog.Api.IntegrationTests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().WithImage("postgres:latest").WithDatabase("blog-db").Build();

        private DbConnection _connection = default!;

        private Respawner _respawner = default!;

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_connection);
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            _connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await _connection.OpenAsync();
            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = [new Table("__EFMigrationsHistory")],
            });
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}
