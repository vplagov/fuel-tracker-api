using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class FuelEntryController(FuelEntryService fuelEntryService) : BaseController
{
    [HttpPost("cars/{carId:guid}/fuel-entries")]
    public async Task<ActionResult<FuelEntryResponse>> Add(Guid carId, FuelEntryRequest request)
    {
        var result = await fuelEntryService.Add(carId, request);
        return result.IsFailure ? 
            HandleFailure(result) : 
            StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpGet("cars/{carId:guid}/fuel-entries")]
    public async Task<ActionResult<IList<FuelEntryResponse>>> GetFuelEntries(Guid carId)
    {
        var result = await fuelEntryService.GetFuelEntries(carId);
        return result.IsFailure ? 
            HandleFailure(result) : 
            Ok(result.Value);
    }

    [HttpDelete("fuel-entries/{fuelEntryId:guid}")]
    public async Task<ActionResult> Delete(Guid fuelEntryId)
    {
        var result = await fuelEntryService.Remove(fuelEntryId);
        return result.IsFailure ? 
            HandleFailure(result) : 
            NoContent();
    }

    [HttpPut("fuel-entries/{fuelEntryId:guid}")]
    public async Task<ActionResult> Update(Guid fuelEntryId, FuelEntryRequest request)
    {
        var result = await fuelEntryService.Update(fuelEntryId, request);
        return result.IsFailure ? 
            HandleFailure(result) : 
            Ok(result.Value);
    }
}