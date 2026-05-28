using FluentResults;
using Mapster;
using User.Application.DTOs.Request;
using User.Application.DTOs.Response;
using User.Application.Interfaces.Repositories;
using User.Application.Interfaces.Services;

namespace User.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> AddAsync(CreateUserRequestDTO request, CancellationToken cancellationToken = default)
    {
        Domain.Entities.User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user != null)
            return Result.Fail("E-mail já está cadastrado.");

        user = request.Adapt<Domain.Entities.User>();

        await _userRepository.AddAsync(user, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Domain.Entities.User? user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return Result.Fail("Usuário não encontrado.");

        await _userRepository.DeleteAsync(user, cancellationToken);

        return Result.Ok();
    }

    public async Task<UserResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        Domain.Entities.User? user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user?.Adapt<UserResponseDTO>();
    }

    public async Task<PageDTO<UserResponseDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        int totalRecords = await _userRepository.CountAsync(cancellationToken);
        IEnumerable<Domain.Entities.User> users = await _userRepository.GetAllAsync(page, pageSize, cancellationToken);

        return new PageDTO<UserResponseDTO>(
            PageNumber: page,
            PageSize: pageSize,
            TotalPages: (int)Math.Ceiling((double)totalRecords / pageSize),
            TotalRecords: totalRecords,
            Data: users.Adapt<List<UserResponseDTO>>()
        );
    }
}