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

    public async Task AddAsync(CreateUserRequestDTO request, CancellationToken cancellationToken = default)
    {
        Domain.Entities.User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user != null)
            throw new InvalidOperationException("Email already exists.");

        user = new Domain.Entities.User
        {
            Name = request.Name,
            Email = request.Email,
            BirthDate = request.BirthDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
    }
}