namespace User.Api.Models;

public sealed record CreateUserModel(
    string? Name,
    string? Email,
    DateOnly? BirthDate
);
