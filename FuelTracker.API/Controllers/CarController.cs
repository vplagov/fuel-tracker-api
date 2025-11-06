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
        
        return Created($"/api/cars/{carResponse.Id}", carResponse); // TODO replace with CreatedAtAction when the GetCar action is available
    }
}