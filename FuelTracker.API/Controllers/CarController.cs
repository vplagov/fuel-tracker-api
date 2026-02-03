using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/cars")]
public class CarController(CarService carService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CarResponse>> AddCar(CarRequest carRequest)
    {
        var carResponse = await carService.Add(carRequest);
        
        return CreatedAtAction(nameof(GetCar), new { id = carResponse.Id }, carResponse);
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
        var result = await carService.GetCar(id);
        return result.IsFailure ? 
            HandleFailure(result) : 
            Ok(result.Value);
    }
}