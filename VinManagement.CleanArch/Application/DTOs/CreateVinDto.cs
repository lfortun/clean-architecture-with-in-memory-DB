namespace Application.DTOs;

public record CreateVinDto(
    string Code,
    string VehicleMake,
    string VehicleModel,
    int? Year);
