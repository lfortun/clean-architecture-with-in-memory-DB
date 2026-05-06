namespace Application.DTOs;

public record VinResponseDto(
    int Id,
    string Code,
    string VehicleMake,
    string VehicleModel,
    int? Year,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
