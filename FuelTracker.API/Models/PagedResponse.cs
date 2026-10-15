namespace FuelTracker.API.Models;

public record PagedResponse<T>(
    IList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
