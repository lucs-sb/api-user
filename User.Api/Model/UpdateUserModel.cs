namespace User.Api.Model;

public sealed record UpdateUserModel(string? Name, string? Email, DateOnly? BirthDate);
