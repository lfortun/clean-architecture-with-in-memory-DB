using Application.DTOs;

namespace Application.Services;

public interface IVinService
{
    Task<VinResponseDto> CreateAsync(CreateVinDto dto, CancellationToken cancellationToken = default);
    Task<VinResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VinResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<VinResponseDto> UpdateAsync(int id, UpdateVinDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
