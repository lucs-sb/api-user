using FluentResults;
using User.Application.DTOs.Request;
using User.Application.DTOs.Response;

namespace User.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> AddAsync(CreateUserRequestDTO request, CancellationToken cancellationToken = default);
    Task<PageDTO<UserResponseDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}