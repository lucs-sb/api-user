using User.Application.DTOs.Request;

namespace User.Application.Interfaces.Services;

public interface IUserService
{
    Task AddAsync(CreateUserRequestDTO request, CancellationToken cancellationToken = default);
}