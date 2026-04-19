using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FuelTracker.API.Models;

namespace FuelTracker.Api.Tests.Integration.Controllers;

public class FuelEntryControllerTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<Guid> CreateCarAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/cars", new CarRequest(name));
        var car = await response.Content.ReadFromJsonAsync<CarResponse>();
        return car!.Id;
    }

    [Fact]
    public async Task Add_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var request = new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 1000, 50, 1.5m, true);

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var fuelEntry = await response.Content.ReadFromJsonAsync<FuelEntryResponse>();
        fuelEntry.Should().NotBeNull();
        fuelEntry!.Odometer.Should().Be(request.Odometer);
        fuelEntry.Liters.Should().Be(request.Liters);
    }

    [Fact]
    public async Task Add_ShouldReturnNotFound_WhenCarDoesNotExist()
    {
        // Arrange
        var nonExistentCarId = Guid.NewGuid();
        var request = new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 1000, 50, 1.5m, true);

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cars/{nonExistentCarId}/fuel-entries", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFuelEntries_ShouldReturnFuelEntries_WhenCarExists()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var request = new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 1000, 50, 1.5m, true);
        await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var entries = await response.Content.ReadFromJsonAsync<List<FuelEntryResponse>>();
        entries.Should().NotBeNull();
        entries.Should().HaveCount(1);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenEntryExists()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var request = new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 1000, 50, 1.5m, true);
        var createResponse = await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);
        var fuelEntry = await createResponse.Content.ReadFromJsonAsync<FuelEntryResponse>();

        // Act
        var response = await Client.DeleteAsync($"/api/fuel-entries/{fuelEntry!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await Client.GetAsync($"/api/cars/{carId}/fuel-entries");
        var entries = await getResponse.Content.ReadFromJsonAsync<List<FuelEntryResponse>>();
        entries.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var request = new FuelEntryRequest(DateOnly.FromDateTime(DateTime.Now), 1000, 50, 1.5m, true);
        var createResponse = await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);
        var fuelEntry = await createResponse.Content.ReadFromJsonAsync<FuelEntryResponse>();
        
        var updateRequest = request with { Odometer = 1100 };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/fuel-entries/{fuelEntry!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedEntry = await response.Content.ReadFromJsonAsync<FuelEntryResponse>();
        updatedEntry!.Odometer.Should().Be(1100);
    }

    [Fact]
    public async Task Add_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var request = new FuelEntryRequest(default, -1, -1, -1, true); // Invalid values

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
