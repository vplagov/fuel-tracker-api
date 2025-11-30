using FuelTracker.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

public class BaseController : ControllerBase
{
    protected ActionResult HandleFailure(Result result)
    {
        switch (result.ErrorType)
        {
            case ErrorType.NotFound:
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: result.ErrorMessage,
                    title: "Not Found");
                
            case ErrorType.Validation:
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: result.ErrorMessage,
                    title: "Validation Error");
                
            case ErrorType.Failure:
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: result.ErrorMessage,
                    title: "Internal Server Error");
                
            default:
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: "An unexpected error occurred.",
                    title: "Server Error");
        }
    }
}