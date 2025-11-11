using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Route("api/cars")]
public class CarController(CarService carService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CarResponse>> AddCar(CarRequest carRequest)
    {
        var carResponse = await carService.Add(carRequest);
        
        return CreatedAtAction(nameof(GetCar), carResponse);
    }

    [HttpGet]
    public async Task<ActionResult<List<CarResponse>>> GetCars()
    {
        var cars = await carService.GetCars();
        return Ok(cars);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CarResponse>> GetCar(Guid id)
    {
        var carResponse = await carService.GetCar(id);
        if (carResponse == null)
        {
            return NotFound($"Car with id '{id}' not found");
        }
        return Ok(carResponse);
    }
}