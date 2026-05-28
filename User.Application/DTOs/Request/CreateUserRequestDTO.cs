namespace User.Application.DTOs.Request;

public sealed record CreateUserRequestDTO(
    string Name,
    string Email,
    DateOnly BirthDate
);