using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Route("api/cars")]
public class FuelEntryController(FuelEntryService fuelEntryService) : ControllerBase
{
    [HttpPost("{carId:guid}/fuel-entries")]
    public async Task<ActionResult<FuelEntryResponse>> Add(Guid carId, FuelEntryRequest request)
    {
        var fuelEntryResponse = await fuelEntryService.Add(carId, request);
        if (fuelEntryResponse == null)
        {
            return NotFound($"Car with ID '{carId}' not found");
        }

        return Created($"/api/cars/{carId}/fuel-entries/{fuelEntryResponse.Id}", fuelEntryResponse);
    }
}