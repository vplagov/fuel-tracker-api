using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FuelTracker.API.Models;

namespace FuelTracker.Api.Tests.Integration.Controllers;

public class CarControllerTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddCar_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CarRequest("Random Car Name");

        // Act
        var response = await Client.PostAsJsonAsync("/api/cars", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var carResponse = await response.Content.ReadFromJsonAsync<CarResponse>();
        carResponse.Should().NotBeNull();
        carResponse.Name.Should().Be(request.Name);
        carResponse.Id.Should().NotBeEmpty();

        // Act: Fetch the car from the GET endpoint
        var getResponse = await Client.GetAsync($"/api/cars/{carResponse.Id}");

        // Assert: Verify it was indeed saved
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getCarResponse = await getResponse.Content.ReadFromJsonAsync<CarResponse>();
        getCarResponse.Should().NotBeNull();
        getCarResponse.Id.Should().Be(carResponse.Id);
        getCarResponse.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetCars_ShouldReturnAllCars_WhenCarsExist()
    {
        // Arrange
        var car1 = new CarRequest("Car 1");
        var car2 = new CarRequest("Car 2");
        await Client.PostAsJsonAsync("/api/cars", car1);
        await Client.PostAsJsonAsync("/api/cars", car2);

        // Act
        var response = await Client.GetAsync("/api/cars");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cars = await response.Content.ReadFromJsonAsync<List<CarResponse>>();
        cars.Should().NotBeNull();
        cars.Should().HaveCount(2);
        cars.Should().Contain(c => c.Name == car1.Name);
        cars.Should().Contain(c => c.Name == car2.Name);
    }

    [Fact]
    public async Task AddCar_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CarRequest("");

        // Act
        var response = await Client.PostAsJsonAsync("/api/cars", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Act: Verify no car was saved
        var getResponse = await Client.GetAsync("/api/cars");
        var cars = await getResponse.Content.ReadFromJsonAsync<List<CarResponse>>();
        cars.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCar_ShouldReturnNotFound_WhenCarDoesNotExist()
    {
        // Arrange
        var request = new CarRequest("Existing Car");
        await Client.PostAsJsonAsync("/api/cars", request);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/cars/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
