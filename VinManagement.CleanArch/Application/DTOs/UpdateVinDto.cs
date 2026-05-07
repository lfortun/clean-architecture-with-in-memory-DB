namespace Application.DTOs;

public record UpdateVinDto(
    string Code,
    string VehicleMake,
    string VehicleModel,
    int? Year);
