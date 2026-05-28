namespace User.Api.Models;

public sealed record UpdateUserModel(
    string? Name,
    string? Email,
    DateOnly? BirthDate
);