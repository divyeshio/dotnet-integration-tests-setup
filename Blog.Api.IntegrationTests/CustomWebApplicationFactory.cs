using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace Blog.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                                                            .WithImage("postgres:latest")
                                                            .WithDatabase("blog-db")
                                                            .Build();

    public HttpClient HttpClient { get; private set; } = default!;

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
        HttpClient = CreateClient();
        await _connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = [new Table("__EFMigrationsHistory")],
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove AppDbContext
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            var connectionString = _dbContainer.GetConnectionString();
            // Add DB context pointing to test container
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            context.Posts.AddRange(new BlogPost { Content = "", Title = "" });
            context.SaveChanges();

        });

    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
