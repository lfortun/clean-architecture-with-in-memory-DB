using Domain.Entities;

namespace Domain.Repositories;

public interface IVinRepository
{
    Task<Vin> CreateAsync(Vin vin, CancellationToken cancellationToken = default);
    Task<Vin?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vin>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Vin> UpdateAsync(Vin vin, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
