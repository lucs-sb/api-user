namespace User.Api.Model;

public sealed record CreateUserModel(
    string? Name,
    string? Email,
    DateOnly? BirthDate
);
