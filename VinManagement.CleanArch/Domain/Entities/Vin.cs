using Domain.Common;

namespace Domain.Entities;

public class Vin : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string VehicleMake { get; private set; } = string.Empty;
    public string VehicleModel { get; private set; } = string.Empty;
    public int? Year { get; private set; }

    private Vin()
    {
    }

    public Vin(string code, string vehicleMake, string vehicleModel, int? year)
    {
        Code = code;
        VehicleMake = vehicleMake;
        VehicleModel = vehicleModel;
        Year = year;
    }

    public void Update(string code, string vehicleMake, string vehicleModel, int? year)
    {
        Code = code;
        VehicleMake = vehicleMake;
        VehicleModel = vehicleModel;
        Year = year;
        UpdatedAt = DateTime.UtcNow;
    }
}
