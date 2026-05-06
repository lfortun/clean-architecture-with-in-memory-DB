using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;

namespace Application.Services;

public class VinService : IVinService
{
    private readonly IVinRepository _repository;
    private readonly IEnumerable<IVinValidator> _validators;

    public VinService(IVinRepository repository, IEnumerable<IVinValidator> validators)
    {
        _repository = repository;
        _validators = validators;
    }

    public async Task<VinResponseDto> CreateAsync(CreateVinDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateVinCodeAsync(dto.Code, cancellationToken);

        if (await _repository.CodeExistsAsync(dto.Code, cancellationToken))
        {
            throw new BadRequestException($"VIN code '{dto.Code}' already exists.");
        }

        var vin = new Vin(dto.Code, dto.VehicleMake, dto.VehicleModel, dto.Year);
        var created = await _repository.CreateAsync(vin, cancellationToken);
        return ToResponseDto(created);
    }

    public async Task<VinResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vin = await _repository.GetByIdAsync(id, cancellationToken);
        if (vin == null)
        {
            throw new NotFoundException(nameof(Vin), id);
        }
        return ToResponseDto(vin);
    }

    public async Task<IReadOnlyList<VinResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vins = await _repository.GetAllAsync(cancellationToken);
        return vins.Select(ToResponseDto).ToList();
    }

    public async Task<VinResponseDto> UpdateAsync(int id, UpdateVinDto dto, CancellationToken cancellationToken = default)
    {
        var vin = await _repository.GetByIdAsync(id, cancellationToken);
        if (vin == null)
        {
            throw new NotFoundException(nameof(Vin), id);
        }

        await ValidateVinCodeAsync(dto.Code, cancellationToken);

        vin.Update(dto.Code, dto.VehicleMake, dto.VehicleModel, dto.Year);
        var updated = await _repository.UpdateAsync(vin, cancellationToken);
        return ToResponseDto(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var exists = await _repository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException(nameof(Vin), id);
        }

        await _repository.DeleteAsync(id, cancellationToken);
    }

    private async Task ValidateVinCodeAsync(string code, CancellationToken cancellationToken)
    {
        foreach (var validator in _validators)
        {
            var isValid = await validator.IsValidAsync(code, cancellationToken);
            if (!isValid)
            {
                throw new BadRequestException($"VIN code '{code}' failed validation with {validator.ProviderName}.");
            }
        }
    }

    private static VinResponseDto ToResponseDto(Vin vin)
    {
        return new VinResponseDto(
            vin.Id,
            vin.Code,
            vin.VehicleMake,
            vin.VehicleModel,
            vin.Year,
            vin.CreatedAt,
            vin.UpdatedAt);
    }
}
