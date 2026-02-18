using System.Security.Claims;

namespace FuelTracker.API.Services;

public class UserContextService(IHttpContextAccessor context)
{
    public Guid GetUserId()
    {
        var contextHttpContext = context.HttpContext
                                 ?? throw new InvalidOperationException(
                                     "The HttpContext is null, but it should not be");
        var claim = contextHttpContext.User.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No NameIdentifier claim is found");
        
        return Guid.Parse(claim.Value);
    }
}