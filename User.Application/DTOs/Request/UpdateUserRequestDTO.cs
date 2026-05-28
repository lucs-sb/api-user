namespace User.Application.DTOs.Request;

public sealed record UpdateUserRequestDTO(string Name, string Email, DateOnly BirthDate);
