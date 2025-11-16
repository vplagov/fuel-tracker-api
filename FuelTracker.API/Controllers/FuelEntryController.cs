using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Route("api")]
public class FuelEntryController(FuelEntryService fuelEntryService) : ControllerBase
{
    [HttpPost("cars/{carId:guid}/fuel-entries")]
    public async Task<ActionResult<FuelEntryResponse>> Add(Guid carId, FuelEntryRequest request)
    {
        var fuelEntryResponse = await fuelEntryService.Add(carId, request);
        if (fuelEntryResponse == null)
        {
            return NotFound($"Car with ID '{carId}' not found");
        }

        return Created($"/api/cars/{carId}/fuel-entries/{fuelEntryResponse.Id}", fuelEntryResponse);
    }

    [HttpGet("cars/{carId:guid}/fuel-entries")]
    public async Task<ActionResult<IList<FuelEntryResponse>>> GetFuelEntries(Guid carId)
    {
        var fuelEntries = await fuelEntryService.GetFuelEntries(carId);
        if (fuelEntries == null)
        {
            return NotFound($"Car with ID '{carId}' not found");
        }
        return Ok(fuelEntries);
    }

    [HttpDelete("fuel-entries/{fuelEntryId:guid}")]
    public async Task<ActionResult> Delete(Guid fuelEntryId)
    {
        var isSuccess = await fuelEntryService.Remove(fuelEntryId);
        if (isSuccess)
        {
            return NoContent();
        }
        return NotFound($"Fuel entry with ID '{fuelEntryId}' is not found");
    }
}