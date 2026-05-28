namespace User.Application.DTOs.Response;

public record PageDTO<T>(
    int PageNumber,
    int PageSize,
    int TotalPages,
    int TotalRecords,
    IReadOnlyList<T>? Data
);
