namespace Domain.Services;

public interface IVinValidator
{
    string ProviderName { get; }
    Task<bool> IsValidAsync(string vin, CancellationToken cancellationToken = default);
}
