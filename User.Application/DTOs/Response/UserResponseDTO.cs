namespace User.Application.DTOs.Response;

public sealed record UserResponseDTO(
    int Id,
    string Name,
    string Email,
    DateOnly BirthDate
);
