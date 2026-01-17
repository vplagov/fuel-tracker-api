using FuelTracker.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

public class BaseController : ControllerBase
{
    protected ActionResult HandleFailure(Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => Problem(
                statusCode: StatusCodes.Status404NotFound, 
                detail: result.ErrorMessage,
                title: "Not Found"),
            ErrorType.Validation => Problem(
                statusCode: StatusCodes.Status400BadRequest, 
                detail: result.ErrorMessage,
                title: "Validation Error"),
            ErrorType.Failure => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: result.ErrorMessage, 
                title: "Internal Server Error"),
            ErrorType.Conflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: result.ErrorMessage,
                title: "Resource already exists"),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError, 
                detail: "An unexpected error occurred.",
                title: "Server Error")
        };
    }
}