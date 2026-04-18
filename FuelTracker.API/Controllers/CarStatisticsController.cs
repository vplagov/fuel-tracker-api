using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/cars/{carId:guid}/statistics")]
public class CarStatisticsController(StatisticsService statisticsService) : BaseController
{
    [HttpGet("average-consumption")]
    public async Task<ActionResult<AverageConsumptionResponse>> GetAverageConsumption(Guid carId)
    {
        var result = await statisticsService.GetAverageConsumption(carId);
        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
}
