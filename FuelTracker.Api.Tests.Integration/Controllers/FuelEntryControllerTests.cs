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

    private async Task CreateFuelEntryAsync(Guid carId, DateOnly date, int odometer)
    {
        var request = new FuelEntryRequest(date, odometer, 50, 1.5m, true);
        var response = await Client.PostAsJsonAsync($"/api/cars/{carId}/fuel-entries", request);
        response.EnsureSuccessStatusCode();
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
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
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
        var result = await getResponse.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();
        result!.Items.Should().BeEmpty();
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

    [Fact]
    public async Task GetFuelEntries_ShouldReturnNewestFirst_AndPaginate()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        var today = DateOnly.FromDateTime(DateTime.Now);
        for (var i = 0; i < 12; i++)
        {
            await CreateFuelEntryAsync(carId, today.AddDays(-i), 1000 + i);
        }

        // Act
        var page1Response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries?page=1&pageSize=10");
        var page1 = await page1Response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();

        // Assert
        page1!.Items.Should().HaveCount(10);
        page1.TotalCount.Should().Be(12);
        page1.TotalPages.Should().Be(2);
        page1.Items.First().Odometer.Should().Be(1000);
        page1.Items.Last().Odometer.Should().Be(1009);

        var page2Response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries?page=2&pageSize=10");
        var page2 = await page2Response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();

        page2!.Items.Should().HaveCount(2);
        page2.Items.Select(e => e.Odometer).Should().Equal(1010, 1011);
    }

    [Fact]
    public async Task GetFuelEntries_ShouldReturnEmptyItems_WhenPageOutOfRange()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        await CreateFuelEntryAsync(carId, DateOnly.FromDateTime(DateTime.Now), 1000);

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries?page=5&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();
        result!.Items.Should().BeEmpty();
        result.Page.Should().Be(5);
        result.TotalCount.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetFuelEntries_ShouldReturnZeroTotalPages_WhenNoEntries()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetFuelEntries_ShouldClampPageSize_WhenAboveMaximum()
    {
        // Arrange
        var carId = await CreateCarAsync("Test Car");
        await CreateFuelEntryAsync(carId, DateOnly.FromDateTime(DateTime.Now), 1000);

        // Act
        var response = await Client.GetAsync($"/api/cars/{carId}/fuel-entries?page=1&pageSize=1000");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<FuelEntryResponse>>();
        result!.PageSize.Should().Be(100);
    }
}
