using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FuelTracker.API.Models;

namespace FuelTracker.Api.Tests.Integration.Controllers;

public class CarStatisticsControllerTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<Guid> CreateCarAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/cars", new CarRequest(name));
        var car = await response.Content.ReadFromJsonAsync<CarResponse>();
        return car!.Id;
    }

    [Fact]
    public async Task GetAverageConsumption_ShouldReturnCalculatedValues_WhenSufficientDataExists()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var date = DateOnly.FromDateTime(DateTime.Now);
        
        // #1 Anchor start: 10,000 km, 45L, Full
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(date.AddDays(-10), 10000, 45, 1.5m, true));
        // #2 Partial: 11,200 km, 20L, Partial
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(date.AddDays(-8), 11200, 20, 1.5m, false));
        // #3 Partial: 12,100 km, 35L, Partial
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(date.AddDays(-5), 12100, 35, 1.5m, false));
        // #4 Anchor end: 13,500 km, 48L, Full
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(date, 13500, 48, 1.5m, true));
        // #5 Excluded: 14,200 km, 25L, Partial
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(date.AddDays(2), 14200, 25, 1.5m, false));

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/statistics/average-consumption");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<AverageConsumptionResponse>();
        stats.Should().NotBeNull();
        
        // Total distance: 13,500 - 10,000 = 3,500 km
        stats!.TotalDistanceKm.Should().Be(3500);
        
        // Total liters: 20 + 35 + 48 = 103L (entry #1 excluded)
        stats.TotalLiters.Should().Be(103m);
        
        // Average: (103 / 3,500) * 100 = 2.9428... -> 2.94
        stats.AverageConsumption.Should().Be(2.94m);
        stats.FullFillUpCount.Should().Be(2);
        stats.PartialFillUpCount.Should().Be(3);
    }

    [Fact]
    public async Task GetAverageConsumption_ShouldReturnInsufficientData_WhenFewerThan2FullFillUps()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 10000, 45, 1.5m, true));
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now).AddDays(1), 10100, 10, 1.5m, false));

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/statistics/average-consumption");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<AverageConsumptionResponse>();
        stats.Should().NotBeNull();
        stats!.AverageConsumption.Should().BeNull();
        stats.Message.Should().Contain("At least 2 full fill-up entries are required");
    }

    [Fact]
    public async Task GetAverageConsumption_ShouldReturnNotFound_WhenCarDoesNotExist()
    {
        // Act
        var response = await Client.GetAsync($"/api/cars/{Guid.NewGuid()}/statistics/average-consumption");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
