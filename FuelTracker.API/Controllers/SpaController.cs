using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

public class SpaController : Controller
{
    [HttpGet("/")]
    [HttpGet("/login")]
    [HttpGet("/register")]
    [HttpGet("/cars")]
    [HttpGet("/cars/{**rest}")]
    [HttpGet("/statistics")]
    public IActionResult Index()
    {
        return File("~/index.html", "text/html");
    }
}
