using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record UpdateVinDto(
    [Required][StringLength(17, MinimumLength = 17)] string Code,
    [Required][StringLength(100)] string VehicleMake,
    [Required][StringLength(100)] string VehicleModel,
    int? Year);
