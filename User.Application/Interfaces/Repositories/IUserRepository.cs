namespace User.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Domain.Entities.User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
}
