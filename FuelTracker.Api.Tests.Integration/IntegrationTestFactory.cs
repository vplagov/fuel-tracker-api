using FuelTracker.API.Database;
using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace FuelTracker.Api.Tests.Integration;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public string AuthToken { get; private set; } = string.Empty;
    public Guid TestUserId { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FuelTrackerContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<FuelTrackerContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var serviceScope = Services.CreateScope();
        var fuelTrackerContext = serviceScope.ServiceProvider.GetRequiredService<FuelTrackerContext>();
        await fuelTrackerContext.Database.MigrateAsync();

        var authService = serviceScope.ServiceProvider.GetRequiredService<AuthService>();
        var username = "testuser_" + Guid.NewGuid().ToString()[..8];
        var password = "Password123!";
        
        await authService.RegisterUser(new RegisterRequest(username, password, "test@example.com"));
        var loginResult = await authService.Login(new LoginRequest(username, password));
        if (loginResult.IsFailure)
        {
            throw new Exception($"Failed to login test user: {loginResult.ErrorMessage}");
        }

        ArgumentNullException.ThrowIfNull(loginResult.Value);
        AuthToken = loginResult.Value.Token;
        TestUserId = loginResult.Value.Id;
    }

    public new Task DisposeAsync() => _dbContainer.StopAsync();
}
