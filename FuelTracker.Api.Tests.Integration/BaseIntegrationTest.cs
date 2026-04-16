using System.Net.Http.Headers;
using FuelTracker.API.Database;
using Microsoft.Extensions.DependencyInjection;

namespace FuelTracker.Api.Tests.Integration;

[Collection("Integration Tests")]
public abstract class BaseIntegrationTest(IntegrationTestFactory factory) : IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();
    protected readonly IntegrationTestFactory Factory = factory;

    public virtual async Task InitializeAsync()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Factory.AuthToken);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FuelTrackerContext>();
        
        dbContext.FuelEntries.RemoveRange(dbContext.FuelEntries);
        dbContext.Cars.RemoveRange(dbContext.Cars);
        await dbContext.SaveChangesAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
}
