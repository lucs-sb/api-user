using FluentResults;
using Mapster;
using User.Application.DTOs.Request;
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
}